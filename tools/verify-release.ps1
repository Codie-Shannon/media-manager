[CmdletBinding()]
param(
    [switch]$SkipWorkingTreeCheck,
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$repositoryRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $scriptDirectory ".."))
$solutionPath = Join-Path $repositoryRoot "MediaManager.sln"
$nugetConfigPath = Join-Path $repositoryRoot "NuGet.Config"
$packagesDirectory = Join-Path $repositoryRoot "src\packages"
$packagingScript = Join-Path $repositoryRoot "packaging\build-portable.ps1"

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot "artifacts"
}
$artifactRoot = [System.IO.Path]::GetFullPath($OutputDirectory)
$stageDirectory = Join-Path $artifactRoot "MediaManager-portable-x64"
$zipPath = Join-Path $artifactRoot "MediaManager-portable-x64.zip"
$checksumPath = $zipPath + ".sha256"
$manifestPath = Join-Path $artifactRoot "RELEASE-MANIFEST.txt"

function Write-Gate {
    param([string]$Message)

    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Invoke-Native {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [string[]]$ArgumentList,
        [Parameter(Mandatory = $true)]
        [string]$FailureMessage
    )

    & $FilePath @ArgumentList
    if ($LASTEXITCODE -ne 0) {
        throw "$FailureMessage Exit code: $LASTEXITCODE."
    }
}

function Resolve-MSBuild {
    $command = Get-Command "MSBuild.exe" -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    $vswhere = Join-Path ${env:ProgramFiles(x86)} (
        "Microsoft Visual Studio\Installer\vswhere.exe")
    if (-not (Test-Path -LiteralPath $vswhere)) {
        throw "MSBuild.exe and vswhere.exe were not found."
    }

    $resolved = & $vswhere -latest -products * `
        -requires Microsoft.Component.MSBuild `
        -find "MSBuild\**\Bin\MSBuild.exe" |
        Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($resolved)) {
        throw "Visual Studio MSBuild was not found."
    }

    return $resolved
}

function Get-TrackedFiles {
    $paths = @(& git -C $repositoryRoot ls-files `
        --cached `
        --others `
        --exclude-standard)
    if ($LASTEXITCODE -ne 0) {
        throw "Unable to enumerate tracked files."
    }

    return $paths
}

function Assert-TrackedFilesArePublic {
    $trackedPaths = @(Get-TrackedFiles)
    $forbiddenExtensions = @(
        ".db",
        ".sqlite",
        ".sqlite3",
        ".log",
        ".mmbak",
        ".pdb",
        ".pfx",
        ".key")
    $forbiddenNames = @(
        ".env",
        "metadata-providers.json")
    $forbiddenDirectories = @(
        ".vs",
        "Logs",
        "Backups",
        "Recovery",
        "MetadataCache",
        "SyntheticFixtures")

    $forbidden = foreach ($relativePath in $trackedPaths) {
        $normalised = $relativePath.Replace("\", "/")
        $name = [System.IO.Path]::GetFileName($normalised)
        $extension = [System.IO.Path]::GetExtension($normalised)
        $segments = @($normalised.Split("/"))
        if (($forbiddenExtensions -contains $extension) -or
            ($forbiddenNames -contains $name) -or
            ($segments | Where-Object {
                $forbiddenDirectories -contains $_
            })) {
            $relativePath
        }
    }
    if ($forbidden) {
        throw "Tracked runtime/private artifacts found:`n$($forbidden -join "`n")"
    }

    $textExtensions = @(
        ".config",
        ".cs",
        ".csproj",
        ".gitignore",
        ".json",
        ".md",
        ".props",
        ".ps1",
        ".resx",
        ".settings",
        ".sln",
        ".targets",
        ".txt",
        ".xaml",
        ".xml",
        ".yaml",
        ".yml")
    $secretPatterns = @(
        "(?i)\bgh[pousr]_[A-Za-z0-9]{20,}\b",
        "(?i)\bgithub_pat_[A-Za-z0-9_]{20,}\b",
        "\bAKIA[0-9A-Z]{16}\b",
        "\bsk-[A-Za-z0-9_-]{20,}\b",
        "\beyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\b")
    $privatePathPattern = "(?i)[A-Z]:\\Users\\[^\\\r\n]+"
    $matches = New-Object System.Collections.Generic.List[string]

    foreach ($relativePath in $trackedPaths) {
        $fullPath = Join-Path $repositoryRoot $relativePath
        $name = [System.IO.Path]::GetFileName($relativePath)
        $extension = [System.IO.Path]::GetExtension($relativePath)
        if (($textExtensions -notcontains $extension) -and
            ($name -ne ".gitignore")) {
            continue
        }

        $content = [System.IO.File]::ReadAllText($fullPath)
        if ([regex]::IsMatch($content, $privatePathPattern)) {
            $matches.Add("${relativePath}: private Windows user path")
        }
        foreach ($pattern in $secretPatterns) {
            if ([regex]::IsMatch($content, $pattern)) {
                $matches.Add("${relativePath}: credential-shaped value")
                break
            }
        }
    }

    if ($matches.Count -gt 0) {
        throw "Privacy/secret scan failed:`n$($matches -join "`n")"
    }
}

function Assert-ReleaseManifest {
    if (-not (Test-Path -LiteralPath $manifestPath)) {
        throw "External release manifest was not produced."
    }

    $stageManifestPath = Join-Path $stageDirectory "RELEASE-MANIFEST.txt"
    if (-not (Test-Path -LiteralPath $stageManifestPath)) {
        throw "The portable stage does not contain RELEASE-MANIFEST.txt."
    }

    $externalHash = (Get-FileHash -Algorithm SHA256 `
        -LiteralPath $manifestPath).Hash
    $stageHash = (Get-FileHash -Algorithm SHA256 `
        -LiteralPath $stageManifestPath).Hash
    if ($externalHash -ne $stageHash) {
        throw "External and packaged release manifests differ."
    }

    $manifestEntries = @{}
    foreach ($line in Get-Content -LiteralPath $stageManifestPath) {
        if ($line -notmatch "^([A-F0-9]{64})  (.+)  ([0-9]+)$") {
            throw "Malformed release-manifest line: $line"
        }

        $manifestEntries[$Matches[2].Replace("\", "/")] = @{
            Hash = $Matches[1]
            Length = [long]$Matches[3]
        }
    }

    $stageFiles = @(Get-ChildItem -LiteralPath $stageDirectory -File -Recurse)
    if ($manifestEntries.Count -ne ($stageFiles.Count - 1)) {
        throw "Release manifest does not cover every staged payload file."
    }

    foreach ($relativePath in $manifestEntries.Keys) {
        $fullPath = Join-Path $stageDirectory $relativePath.Replace(
            "/",
            [System.IO.Path]::DirectorySeparatorChar)
        if (-not (Test-Path -LiteralPath $fullPath)) {
            throw "Release manifest references a missing file: $relativePath"
        }

        $item = Get-Item -LiteralPath $fullPath
        $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $fullPath).Hash
        if (($hash -ne $manifestEntries[$relativePath].Hash) -or
            ($item.Length -ne $manifestEntries[$relativePath].Length)) {
            throw "Release manifest mismatch: $relativePath"
        }
    }
}

function Assert-ZipAndChecksum {
    if (-not (Test-Path -LiteralPath $zipPath)) {
        throw "Portable ZIP was not produced."
    }
    if (-not (Test-Path -LiteralPath $checksumPath)) {
        throw "Portable checksum was not produced."
    }

    $zipHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $zipPath).Hash
    $checksumLine = (Get-Content -LiteralPath $checksumPath -Raw).Trim()
    $expectedLine = "{0}  {1}" -f $zipHash, (
        [System.IO.Path]::GetFileName($zipPath))
    if ($checksumLine -ne $expectedLine) {
        throw "Portable ZIP checksum file does not match the ZIP."
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
    try {
        $entries = @{}
        foreach ($entry in $archive.Entries) {
            if ([string]::IsNullOrEmpty($entry.Name)) {
                continue
            }
            $entries[$entry.FullName.Replace("\", "/")] = $entry
        }

        $stageFiles = @(Get-ChildItem -LiteralPath $stageDirectory -File -Recurse)
        if ($entries.Count -ne $stageFiles.Count) {
            throw "ZIP entry count does not match the portable stage."
        }

        foreach ($file in $stageFiles) {
            $relativePath = $file.FullName.Substring(
                $stageDirectory.Length).TrimStart("\").Replace("\", "/")
            if (-not $entries.ContainsKey($relativePath)) {
                throw "ZIP is missing staged file: $relativePath"
            }

            $stream = $entries[$relativePath].Open()
            try {
                $sha = [System.Security.Cryptography.SHA256]::Create()
                try {
                    $entryHash = [System.BitConverter]::ToString(
                        $sha.ComputeHash($stream)).Replace("-", "")
                }
                finally {
                    $sha.Dispose()
                }
            }
            finally {
                $stream.Dispose()
            }

            $fileHash = (Get-FileHash -Algorithm SHA256 `
                -LiteralPath $file.FullName).Hash
            if ($entryHash -ne $fileHash) {
                throw "ZIP content mismatch: $relativePath"
            }
        }
    }
    finally {
        $archive.Dispose()
    }

    return $zipHash
}

$msbuildPath = Resolve-MSBuild

Push-Location $repositoryRoot
try {
    Write-Gate "Restore NuGet packages"
    $nuget = Get-Command "nuget.exe" -ErrorAction SilentlyContinue
    if ($nuget) {
        Invoke-Native -FilePath $nuget.Source -ArgumentList @(
            "restore",
            $solutionPath,
            "-ConfigFile",
            $nugetConfigPath,
            "-PackagesDirectory",
            $packagesDirectory,
            "-NonInteractive") -FailureMessage "NuGet restore failed."
    }
    else {
        Write-Host "NuGet.exe was not found; using MSBuild packages.config restore."
        Invoke-Native -FilePath $msbuildPath -ArgumentList @(
            $solutionPath,
            "/t:Restore",
            "/p:RestorePackagesConfig=true",
            "/p:RestoreRepositoryPath=$packagesDirectory",
            "/p:RestoreConfigFile=$nugetConfigPath",
            "/m",
            "/verbosity:minimal") -FailureMessage "MSBuild restore failed."
    }

    foreach ($configuration in @("Debug", "Release")) {
        Write-Gate "Rebuild $configuration x64"
        Invoke-Native -FilePath $msbuildPath -ArgumentList @(
            $solutionPath,
            "/t:Rebuild",
            "/p:Configuration=$configuration",
            "/p:Platform=x64",
            "/m",
            "/verbosity:minimal") -FailureMessage (
                "$configuration x64 rebuild failed.")

        Write-Gate "Run $configuration stability suite"
        $testExecutable = Join-Path $repositoryRoot (
            "tests\MediaManager.StabilityTests\bin\x64\$configuration\" +
            "MediaManager.StabilityTests.exe")
        if (-not (Test-Path -LiteralPath $testExecutable)) {
            throw "Stability executable was not produced: $testExecutable"
        }
        Invoke-Native -FilePath $testExecutable -ArgumentList @() `
            -FailureMessage "$configuration stability suite failed."
    }

    Write-Gate "Build privacy-gated portable package"
    & $packagingScript -Configuration Release -Platform x64 `
        -OutputDirectory $artifactRoot
    if ($LASTEXITCODE -ne 0) {
        throw "Portable packaging failed with exit code $LASTEXITCODE."
    }

    Write-Gate "Scan tracked files for private/runtime data"
    Assert-TrackedFilesArePublic

    Write-Gate "Verify release manifest, ZIP, and checksum"
    Assert-ReleaseManifest
    $zipHash = Assert-ZipAndChecksum

    if (-not $SkipWorkingTreeCheck) {
        Write-Gate "Verify clean Git working tree"
        $status = @(& git -C $repositoryRoot status --porcelain `
            --untracked-files=all)
        if ($LASTEXITCODE -ne 0) {
            throw "Unable to inspect the Git working tree."
        }
        if ($status.Count -gt 0) {
            throw "Git working tree is not clean:`n$($status -join "`n")"
        }
    }

    Write-Host ""
    Write-Host "PASS: complete Media Manager release gate" `
        -ForegroundColor Green
    Write-Host "Portable ZIP: $zipPath"
    Write-Host "SHA-256: $zipHash"
}
finally {
    Pop-Location
}
