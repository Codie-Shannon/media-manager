# Build status

Date: 2026-07-26

## Verified

- NuGet package restore succeeds using the repository `NuGet.Config`.
- The complete `MediaManager.sln` rebuilds in Debug x64 and Release x64 with zero errors.
- `MediaControlsLibrary`, `MediaControlsTester`, and `Media_Manager` build from their repository project relationships.
- `MediaManager.StabilityTests` builds and passes in Debug x64 and Release x64.
- `Media_Manager` launches from both Debug x64 and Release x64 output.
- The same restore, builds, and launches succeed from a fresh tracked-file checkout with no inherited `packages`, `bin`, or `obj` directories.
- A fresh Windows profile creates `%LOCALAPPDATA%\Media_Manager`, `MediaManagerDB.db`, and all eight required image directories automatically.
- A controlled exception raised after managed startup begins displays `Media Manager Startup Error`, its root-cause details, and a working Close button.
- An isolated Group 3 profile passes the existing-functionality matrix without modifying the real user database.

## Restored dependency relationship

The recovered project previously referenced a generated DLL through a path outside the repository:

```text
..\..\..\Project Files\Projects\MediaControlsLibrary\MediaControlsLibrary\bin\x64\Debug\MediaControlsLibrary.dll
```

Group 2 replaced it with a project reference to `src/MediaControlsLibrary/MediaControlsLibrary.csproj`. The application no longer depends on stale generated output.

## Restored startup

Database initialization now validates its data path, creates the directory, and combines the database filename safely. Application startup is explicit so managed construction failures can be shown without relying on application resources or the controls library.

Failures that occur before managed WPF startup, such as Windows being unable to load the executable or CLR, remain operating-system loader errors.

## Known compiler-warning themes

- The controls library reports two warnings for unused `SearchBoxBase` fields.
- inherited property hiding in TV-show/season models;
- unawaited tasks;
- `async` methods without `await`;
- unused error strings.

Warnings are retained in the baseline and will be triaged during restoration.

The verified full-solution rebuild currently reports the recovered warning set and zero errors in each configuration. Group 3 introduces no new compiler warnings.

## Dependency audit notices

Package restore reports known advisories for the recovered versions of:

- `AngleSharp` 1.0.5 - moderate severity;
- `System.Text.Json` 7.0.3 - high severity.

The baseline preserves recovered package versions for traceability. Upgrading or removing unused packages is required before release.
