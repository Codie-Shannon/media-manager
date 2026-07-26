# Recovered build status

Date: 2026-07-26

## Verified

- NuGet package restore succeeds using the repository `NuGet.Config`.
- `MediaControlsLibrary`: Debug x64 and Release x64 builds succeed from this curated repository.
- `MediaControlsTester`: Debug x64 and Release x64 builds succeed from this curated repository, and the tester launches.
- The recovered `Media_Manager` executable launches and displays the full shell when `%LOCALAPPDATA%\Media_Manager` exists.
- An isolated application build succeeds when the recovered controls-library output is supplied explicitly as a reference path.

## Not yet reproducible

The application project contains a stale file reference:

```text
..\..\..\Project Files\Projects\MediaControlsLibrary\MediaControlsLibrary\bin\x64\Debug\MediaControlsLibrary.dll
```

That path does not exist in the curated repository. Existing recovery output could appear to build because an old DLL remained in `bin`; generated output is intentionally excluded here.

Group 2 will replace the file reference with a project reference to `src/MediaControlsLibrary/MediaControlsLibrary.csproj`.

## Startup issue

On a fresh profile, startup throws `DirectoryNotFoundException` because SQLite attempts to create `MediaManagerDB.db` before the application creates `%LOCALAPPDATA%\Media_Manager`.

Creating that directory externally allows the recovered executable to launch. Group 2 will make startup create its own required directories and report failures clearly.

## Known compiler-warning themes

- The controls library reports two warnings for unused `SearchBoxBase` fields.
- inherited property hiding in TV-show/season models;
- unawaited tasks;
- `async` methods without `await`;
- unused error strings.

Warnings are retained in the baseline and will be triaged during restoration.

## Dependency audit notices

Package restore reports known advisories for the recovered versions of:

- `AngleSharp` 1.0.5 - moderate severity;
- `System.Text.Json` 7.0.3 - high severity.

The baseline preserves recovered package versions for traceability. Upgrading or removing unused packages is required before release.
