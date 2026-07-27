# Media Manager v1.0.0 release

`v1.0.0` is the portfolio closure release for the recovered and modernised Media Manager origin project.

## Release contents

- Release x64 WPF application for .NET Framework 4.7.2;
- recovered reusable `MediaControlsLibrary`;
- supported TMDB/IGDB metadata-provider boundary;
- local database, backup, restore, recovery, health-check, export, and log behavior;
- disposable synthetic `--demo` mode;
- original, restored, modern, and README contrast evidence;
- automated stability executable in the solution;
- repeatable portable ZIP pipeline and checksum.

## Portable build

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File packaging\build-portable.ps1
```

Outputs:

```text
artifacts\MediaManager-portable-x64\
artifacts\MediaManager-portable-x64.zip
artifacts\MediaManager-portable-x64.zip.sha256
```

Run `Media_Manager.exe --demo` from the extracted folder for an isolated walkthrough. The package contains no user database, real library, provider credentials, cache, log, backup, recovery copy, or debug symbol.

## Verification record

The final Group 7 build record, ZIP SHA-256, file count, smoke-test outcome, real-database hash check, and privacy scan are recorded in [build-status.md](build-status.md). The checksum file beside the ZIP remains the authoritative value for a locally rebuilt artifact.

## Distribution notes

- The package is unsigned and distributed as a portable ZIP, not an installer.
- Windows may show an unrecognised-publisher warning.
- Provider credentials are optional and entered by the user after launch.
- The application source currently has no reuse license; third-party components retain their own licenses and notices.
- No GitHub Release binary is committed to the repository. The tag and reproducible packaging command identify the source release.

## Public project copy

GitHub description:

> Privacy-first C# WPF media library organiser - restored from my diploma-era origin project and modernized with supported metadata providers.

Portfolio card:

> Media Manager - Diploma origin project rebuilt as a modern, privacy-first C# WPF desktop application.

Case-study summary:

> Media Manager was the first serious software application I built. It taught me how desktop applications, custom controls, data, files, metadata, persistence, and UI workflows fit together. After recovering the project from a corrupt drive, I restored its control library, replaced brittle scraping with a supported provider architecture, verified the original workflows, and then redesigned the application as a modern Windows desktop product.
