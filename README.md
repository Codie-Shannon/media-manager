# Media Manager

Media Manager is a local-first Windows desktop application for organizing movies, TV shows, videos, pictures, music, and games. This repository is a curated restoration of an original C#/WPF project and its reusable controls library.

## Current status

The repository currently records the recovered baseline before application restoration:

- `MediaControlsLibrary` builds and its tester launches.
- The recovered Media Manager executable can launch after its LocalAppData directory exists.
- A clean application build is not yet reproducible because the app references a stale controls DLL by a broken path.
- Metadata retrieval still contains legacy direct-site automation and has not yet been replaced by a provider abstraction.
- No real media library, user database, credentials, or private paths are included.

See [docs/CURRENT_GROUP.md](docs/CURRENT_GROUP.md) and [docs/build-status.md](docs/build-status.md) for the precise state.

## Repository layout

- `src/Media_Manager` - recovered full application source
- `src/MediaControlsLibrary` - recovered WPF controls library
- `src/MediaControlsTester` - controls demonstration application
- `docs` - restoration plan, architecture notes, evidence, and screenshots
- `tests` - future automated test projects
- `sample-data` - future synthetic fixtures only
- `packaging` - future portable build or installer work

## Build prerequisites

- Windows
- Visual Studio 2022 with .NET desktop development
- .NET Framework 4.7.2 developer pack
- NuGet package restore

Open `MediaManager.sln`. The controls projects are the currently verified build targets. Restoring the full application build is Group 2 work.

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

