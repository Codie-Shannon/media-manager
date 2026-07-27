# Build status

Date: 2026-07-27

## Verified

- NuGet package restore succeeds using the repository `NuGet.Config`, which declares nuget.org explicitly instead of relying on machine-level feed configuration.
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
- The Group 5 draft `packaging/build-portable.ps1` produced a 51-file Release x64 portable folder, ZIP, and SHA-256 checksum with zero database, credential, log, cache, recovery, or personal-data artifacts.
- The packaged demo remains responsive, starts zero new Chrome processes, and leaves the real database SHA-256 unchanged.
- Group 6 Debug x64 rebuilds with zero errors and the complete stability executable passes.
- The synthetic demo verifies the modern shell, six libraries, cards/details, empty state, settings, add forms, metadata-search state, and exact 13-image evidence set.
- Group 7 adds three visually checked README contrast composites sourced from the fixed 13-image screenshot groups.
- A 42-second H.264 modern-interface walkthrough was checked at seven timestamps and contains only the synthetic application window.
- All 34 local Markdown links resolve.
- The tracked-file audit finds zero common credential patterns, absolute personal paths, databases, logs, backups, keys, certificates, or local provider-setting files.
- Active runtime, demo-harness, provider-attribution, and asset provenance were reviewed and recorded in `docs/licensing.md` and `THIRD-PARTY-NOTICES.md`.
- A current staged-source clean checkout restores from the declared nuget.org feed, rebuilds Debug x64 and Release x64 with zero errors, passes both stability executables, and produces the portable package.
- The final local v1.0.0 portable stage contains 55 files and 25,548,754 bytes, including 54 entries in its per-file SHA-256 manifest.
- The final local ZIP SHA-256 is `7C0C01E005C9A022C4914A5921557AAB179BF5D39D6C3AFB7C0C0E32292C8306`.
- The packaged v1.0.0 `--demo` launches responsively, starts zero new Chrome processes, contains zero forbidden runtime/private artifacts, and leaves the real user database byte-for-byte unchanged.
- The real database SHA-256 remains `885446BEF1FDCB43678DE1D9542C3519F516AD0EB8E5B1055F5725D54957DB85`.

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

The verified full-solution rebuild currently reports the recovered warning set and zero errors in each configuration. Groups 4-7 introduce no new compiler-warning category.

## Dependency audit notices

`AngleSharp` was used only by the removed legacy scraping stack and is no longer referenced by the application. Group 5 also removes the unused `System.Text.Json` 7.0.3 reference and package, so its high-severity advisory is no longer present in application runtime output.
