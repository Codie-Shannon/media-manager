# Media Manager

Media Manager is a local-first Windows desktop application for organizing movies, TV shows, videos, pictures, music, and games. This repository is a curated restoration of an original C#/WPF project and its reusable controls library.

## Current status

Group 6 is complete. The recovered application now combines supported metadata providers and recoverable local persistence with a cohesive modern desktop interface:

- The full solution restores and builds in Debug x64 and Release x64.
- `Media_Manager` references `MediaControlsLibrary` as a project dependency instead of a stale external DLL.
- A fresh profile creates its LocalAppData database directory and required image directories automatically.
- Startup failures raised after managed startup begins are presented in a dependency-free error window.
- An isolated synthetic profile verifies all six libraries, hierarchy navigation, card details, add/edit/remove/delete flows, Explorer actions, playback surfaces, game launch, and resize behavior.
- Critical TV-show ownership, destructive deletion, folder-browser, missing-path, missing-artwork, invalid-date, and game-launch failures are repaired.
- TMDB supplies movie, TV, season, and episode metadata; IGDB supplies game metadata.
- Search and detail retrieval use `IMetadataProvider`, provider-neutral models, cancellation, timeouts, encrypted local credentials, caching, and stale-cache fallback.
- Manual results remain available without credentials or network access.
- Selenium, Chrome-driver, direct IMDb, and direct Metacritic automation are removed from the active application and its runtime output.
- Existing IMDb references can be resolved through TMDB's supported external-ID endpoint without scraping.
- The SQLite connection path is held in memory; startup no longer rewrites the executable configuration file.
- Existing integer primary keys remain stable, and schema version 1 records the reviewed, compatible format without a destructive migration.
- Settings provides verified backup, restore, path-redacted catalog export, and duplicate/missing-path health checks.
- Daily automatic backups retain the newest seven snapshots. Restore creates a safety backup first and rolls back failed replacements.
- Corrupt databases are preserved under the local `Recovery` directory and restored from the newest valid backup when possible.
- Local rolling logs contain startup, background-task, backup, restore, and recovery failures.
- `Media_Manager.exe --demo` creates a disposable synthetic profile with generated covers and neutral library items.
- A repeatable `MediaManager.StabilityTests` executable protects Group 3-5 behavior, including mocked providers, backup/restore, corruption recovery, path redaction, invalid-backup rejection, and a 2,500-record scan.
- `packaging/build-portable.ps1` rebuilds Release x64 and produces a personal-data-free portable folder, ZIP, and SHA-256 checksum.
- Clean tracked-file checkouts build and launch without inherited `bin`, `obj`, or package output.
- No real media library, user database, credentials, or private paths are included.
- Original and restored screenshot groups are preserved as fixed comparison evidence.
- A modern navy/cyan shell now unifies all six libraries, command surfaces, cards, details, forms, settings, provider configuration, dialogs, empty states, and loading states.
- Reusable UI tokens and controls live in `Media_Manager.Controls` while the recovered control library retains the application workflows.
- Keyboard focus is visible, primary controls have meaningful automation names, command surfaces can scroll at constrained widths, and long forms/settings remain reachable.
- The 13-image modern screenshot group provides exact original/restored/modern comparisons using only synthetic data.

The next milestone is Group 7, packaging, portfolio presentation, and project closure.

See [docs/CURRENT_GROUP.md](docs/CURRENT_GROUP.md), [docs/build-status.md](docs/build-status.md), and [docs/ui-resource-inventory.md](docs/ui-resource-inventory.md) for the precise state.

## Repository layout

- `src/Media_Manager` - recovered full application source
- `src/MediaControlsLibrary` - recovered WPF controls library
- `src/MediaControlsTester` - controls demonstration application
- `docs` - restoration plan, architecture notes, evidence, and screenshots
- `tests` - automated stability and future provider/persistence tests
- `sample-data` - generated demo catalog and publication-safe fixture policy
- `packaging` - repeatable portable Release candidate build

## Build prerequisites

- Windows
- Visual Studio 2022 with .NET desktop development
- .NET Framework 4.7.2 developer pack
- NuGet package restore

Open `MediaManager.sln`, restore NuGet packages, and build the `x64` configuration. The complete solution is verified in both Debug and Release.

Run the synthetic demo without accessing a real library:

```powershell
src\Media_Manager\bin\x64\Release\Media_Manager.exe --demo
```

Create the portable Release candidate:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File packaging\build-portable.ps1
```

Provider credentials are entered under **Settings > Configure Metadata Providers** and are encrypted for the current Windows user. Environment-variable setup and provider behavior are documented in [docs/metadata-provider-migration.md](docs/metadata-provider-migration.md).

Backup, restore, health-check, catalog-export, and recovery behavior is documented in [docs/data-recovery.md](docs/data-recovery.md).

## Privacy and sample-data policy

The project is designed to operate locally. This repository must not contain:

- real media libraries or databases;
- personal filesystem paths;
- API credentials;
- copyrighted poster, cover, or game-library samples;
- generated `bin`, `obj`, `.vs`, or NuGet package directories.

Only synthetic sample data and publication-safe evidence may be committed.

## License

No license has been selected yet. Dependency and recovered-asset provenance must be reviewed before a license is added.
