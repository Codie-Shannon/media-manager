# Media Manager

Media Manager is a local-first Windows desktop application for organizing movies, TV shows, videos, pictures, music, and games. This repository is a curated restoration of an original C#/WPF project and its reusable controls library.

## Current status

Group 4 is complete. The recovered application now uses supported, swappable metadata providers instead of browser automation:

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
- A repeatable `MediaManager.StabilityTests` executable protects Group 3 stability plus mocked TMDB/IGDB parsing, cancellation, cache, credential encryption, and manual fallback.
- Clean tracked-file checkouts build and launch without inherited `bin`, `obj`, or package output.
- No real media library, user database, credentials, or private paths are included.
- Original and restored screenshot groups are preserved as fixed comparison evidence.

The next milestone is Group 5 data reliability and release preparation. The visible UI redesign remains Group 6.

See [docs/CURRENT_GROUP.md](docs/CURRENT_GROUP.md), [docs/build-status.md](docs/build-status.md), and [docs/ui-resource-inventory.md](docs/ui-resource-inventory.md) for the precise state.

## Repository layout

- `src/Media_Manager` - recovered full application source
- `src/MediaControlsLibrary` - recovered WPF controls library
- `src/MediaControlsTester` - controls demonstration application
- `docs` - restoration plan, architecture notes, evidence, and screenshots
- `tests` - automated stability and future provider/persistence tests
- `sample-data` - synthetic-fixture policy and future publication-safe fixtures
- `packaging` - future portable build or installer work

## Build prerequisites

- Windows
- Visual Studio 2022 with .NET desktop development
- .NET Framework 4.7.2 developer pack
- NuGet package restore

Open `MediaManager.sln`, restore NuGet packages, and build the `x64` configuration. The complete solution is verified in both Debug and Release.

Provider credentials are entered under **Settings > Configure Metadata Providers** and are encrypted for the current Windows user. Environment-variable setup and provider behavior are documented in [docs/metadata-provider-migration.md](docs/metadata-provider-migration.md).

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
