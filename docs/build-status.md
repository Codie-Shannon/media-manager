# Build status

Date: 2026-07-27

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
- Group 4 removes Selenium and related browser automation from source references and application runtime output.
- Mocked TMDB/IGDB, cancellation, cache, encrypted-setting, and manual-fallback tests pass.
- A supported live IGDB authentication/search smoke test passes without printing or committing credentials.
- The isolated Group 4 application launches with zero new Chrome processes and leaves the real user database unchanged.
- Group 5 keeps SQLite profile paths in memory and records the reviewed compatible format as schema version 1.
- Disposable tests pass consistent backup/restore, managed-cover recovery, corrupt-database recovery, invalid-backup rejection, path-redacted export, and duplicate/missing-path reporting.
- A synthetic library with more than 2,500 records completes the practical health-scan gate.
- Release `--demo` creates five generated items, original synthetic covers, a log, and one automatic backup under a disposable temp profile.
- `packaging/build-portable.ps1` produces a 51-file Release x64 portable folder, ZIP, and SHA-256 checksum with zero database, credential, log, cache, recovery, or personal-data artifacts.
- The packaged demo remains responsive, starts zero new Chrome processes, and leaves the real database SHA-256 unchanged.

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

The verified full-solution rebuild currently reports the recovered warning set and zero errors in each configuration. Groups 4 and 5 introduce no new compiler-warning category.

## Dependency audit notices

`AngleSharp` was used only by the removed legacy scraping stack and is no longer referenced by the application. Group 5 also removes the unused `System.Text.Json` 7.0.3 reference and package, so its high-severity advisory is no longer present in application runtime output.
