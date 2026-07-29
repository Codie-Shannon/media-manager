param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [ValidateSet("x64")]
    [string]$Platform = "x64",
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$repositoryRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $scriptDirectory ".."))

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot "artifacts"
}

$artifactRoot = [System.IO.Path]::GetFullPath($OutputDirectory)
$stageDirectory = Join-Path $artifactRoot "MediaManager-portable-x64"
$zipPath = Join-Path $artifactRoot "MediaManager-portable-x64.zip"
$checksumPath = $zipPath + ".sha256"
$manifestPath = Join-Path $artifactRoot "RELEASE-MANIFEST.txt"
$solutionPath = Join-Path $repositoryRoot "MediaManager.sln"
$releaseDirectory = Join-Path $repositoryRoot (
    "src\Media_Manager\bin\{0}\{1}" -f $Platform, $Configuration)

function Assert-ContainedPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Root
    )

    $resolvedRoot = [System.IO.Path]::GetFullPath($Root).TrimEnd("\") + "\"
    $resolvedPath = [System.IO.Path]::GetFullPath($Path)
    if (-not $resolvedPath.StartsWith(
        $resolvedRoot,
        [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Packaging target escapes the artifact directory: $resolvedPath"
    }
}

Assert-ContainedPath -Path $stageDirectory -Root $artifactRoot
Assert-ContainedPath -Path $zipPath -Root $artifactRoot
Assert-ContainedPath -Path $checksumPath -Root $artifactRoot
Assert-ContainedPath -Path $manifestPath -Root $artifactRoot

$msbuild = Get-Command "MSBuild.exe" -ErrorAction SilentlyContinue
if (-not $msbuild) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} (
        "Microsoft Visual Studio\Installer\vswhere.exe")
    if (-not (Test-Path -LiteralPath $vswhere)) {
        throw "MSBuild.exe and vswhere.exe were not found."
    }

    $msbuildPath = & $vswhere -latest -products * `
        -requires Microsoft.Component.MSBuild `
        -find "MSBuild\**\Bin\MSBuild.exe" |
        Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($msbuildPath)) {
        throw "Visual Studio MSBuild was not found."
    }
}
else {
    $msbuildPath = $msbuild.Source
}

& $msbuildPath $solutionPath /t:Rebuild `
    /p:Configuration=$Configuration `
    /p:Platform=$Platform `
    /m `
    /verbosity:minimal
if ($LASTEXITCODE -ne 0) {
    throw "Release build failed with exit code $LASTEXITCODE."
}

if (-not (Test-Path -LiteralPath (
    Join-Path $releaseDirectory "Media_Manager.exe"))) {
    throw "The application executable was not produced."
}

New-Item -ItemType Directory -Path $artifactRoot -Force | Out-Null
if (Test-Path -LiteralPath $stageDirectory) {
    Remove-Item -LiteralPath $stageDirectory -Recurse -Force
}
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
if (Test-Path -LiteralPath $checksumPath) {
    Remove-Item -LiteralPath $checksumPath -Force
}
if (Test-Path -LiteralPath $manifestPath) {
    Remove-Item -LiteralPath $manifestPath -Force
}

New-Item -ItemType Directory -Path $stageDirectory | Out-Null
Copy-Item -Path (Join-Path $releaseDirectory "*") `
    -Destination $stageDirectory `
    -Recurse `
    -Force

Get-ChildItem -LiteralPath $stageDirectory -File -Recurse |
    Where-Object { $_.Extension -in @(".pdb", ".xml") } |
    Remove-Item -Force

Copy-Item -LiteralPath (Join-Path $repositoryRoot "README.md") `
    -Destination $stageDirectory
Copy-Item -LiteralPath (Join-Path $repositoryRoot "CHANGELOG.md") `
    -Destination $stageDirectory
Copy-Item -LiteralPath (
    Join-Path $repositoryRoot "THIRD-PARTY-NOTICES.md") `
    -Destination $stageDirectory
Copy-Item -LiteralPath (Join-Path $repositoryRoot "docs\release.md") `
    -Destination (Join-Path $stageDirectory "RELEASE-NOTES.md")

$portableReadme = @"
Media Manager v1.0.1 portable build

Run Media_Manager.exe for a normal local profile.
Run Media_Manager.exe --demo for a disposable synthetic demo profile.

Local databases, cover images, logs, provider credentials, and personal
media are never included in this package. They are created at runtime.

This build is unsigned. Media Manager currently grants no source reuse
license. Third-party components retain the licenses listed in
THIRD-PARTY-NOTICES.md.
"@
Set-Content -LiteralPath (
    Join-Path $stageDirectory "PORTABLE-README.txt") `
    -Value $portableReadme `
    -Encoding UTF8
Set-Content -LiteralPath (
    Join-Path $stageDirectory "VERSION.txt") `
    -Value "1.0.1" `
    -Encoding ASCII

$forbidden = Get-ChildItem -LiteralPath $stageDirectory -Force -Recurse |
    Where-Object {
        ($_.PSIsContainer -and $_.Name -in @(
            ".git",
            "Logs",
            "Backups",
            "Recovery",
            "MetadataCache")) -or
        (-not $_.PSIsContainer -and (
            $_.Extension -in @(
                ".db",
                ".sqlite",
                ".sqlite3",
                ".log",
                ".mmbak",
                ".pdb",
                ".pfx",
                ".key") -or
            $_.Name -in @(
                "metadata-providers.json",
                ".env")))
    }
if ($forbidden) {
    throw "Runtime or personal data was found in the portable stage."
}

$privatePathMatches = Get-ChildItem -LiteralPath $stageDirectory -File -Recurse |
    Where-Object {
        $_.Extension -in @(".config", ".json", ".md", ".txt")
    } |
    Select-String -Pattern "[A-Za-z]:\\Users\\" -SimpleMatch:$false
if ($privatePathMatches) {
    throw "A private Windows user path was found in the portable stage."
}

$manifestLines = Get-ChildItem -LiteralPath $stageDirectory -File -Recurse |
    Sort-Object FullName |
    ForEach-Object {
        $relativePath = $_.FullName.Substring(
            $stageDirectory.Length).TrimStart("\")
        $fileHash = (Get-FileHash -Algorithm SHA256 `
            -LiteralPath $_.FullName).Hash
        "{0}  {1}  {2}" -f $fileHash, $relativePath, $_.Length
    }
Set-Content -LiteralPath (
    Join-Path $stageDirectory "RELEASE-MANIFEST.txt") `
    -Value $manifestLines `
    -Encoding ASCII
Copy-Item -LiteralPath (
    Join-Path $stageDirectory "RELEASE-MANIFEST.txt") `
    -Destination $manifestPath

Compress-Archive -Path (Join-Path $stageDirectory "*") `
    -DestinationPath $zipPath `
    -CompressionLevel Optimal

$hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $zipPath).Hash
Set-Content -LiteralPath $checksumPath `
    -Value ("{0}  {1}" -f $hash, (Split-Path -Leaf $zipPath)) `
    -Encoding ASCII

Write-Output "Portable folder: $stageDirectory"
Write-Output "Portable ZIP: $zipPath"
Write-Output "Release manifest: $manifestPath"
Write-Output "SHA-256: $hash"
