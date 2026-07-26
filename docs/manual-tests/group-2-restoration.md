# Group 2 restoration verification

Date: 2026-07-26

## Scope

Verify that the curated full application restores, builds, starts, initializes a fresh profile, and presents managed startup failures without relying on recovered build output.

## Results

| Check | Result |
| --- | --- |
| NuGet restore from the working repository | Pass; two known advisory warnings |
| Full solution Debug x64 rebuild | Pass; 42 warnings, 0 errors |
| Full solution Release x64 rebuild | Pass; 42 warnings, 0 errors |
| Fresh tracked-file checkout restore | Pass |
| Fresh tracked-file checkout Debug x64 rebuild and launch | Pass |
| Fresh tracked-file checkout Release x64 rebuild and launch | Pass |
| Fresh-profile database creation | Pass |
| Fresh-profile required image-directory creation | Pass; all eight directories |
| Managed startup-error presentation | Pass |
| Original LocalAppData profile preserved | Pass |
| Restored screenshot group | Pass; 13 publication-safe images |

## Clean-checkout proof

A temporary checkout was produced from tracked repository content, with the Group 2 source changes overlaid and no pre-existing `packages`, `bin`, or `obj` directories. Package restore and complete Debug x64 and Release x64 rebuilds succeeded. Each resulting `Media_Manager.exe` displayed the `Media Manager` window.

## Fresh-profile proof

The existing `%LOCALAPPDATA%\Media_Manager` directory was moved aside for the test and restored afterward. On first launch the application created:

- `MediaManagerDB.db`
- `Images\Movie Covers`
- `Images\TV Show Covers`
- `Images\Season Covers`
- `Images\Episode Covers`
- `Images\Video Preview`
- `Images\Image Preview`
- `Images\Music Covers`
- `Images\Game Covers`

The original profile, database, synthetic fixtures, and images were restored after verification.

## Startup-error proof

A temporary controlled exception was introduced after managed WPF startup began. The application displayed a dependency-free window titled `Media Manager Startup Error`, included the root-cause message, and provided a working Close button. The temporary exception was then removed and both final configurations were rebuilt successfully.

## Screenshot proof

The 13 restored-state images in `docs/screenshot-groups/restored` mirror the original-state coverage: all six libraries, the add selector, movie/folder/game forms, sort menu, empty game metadata-search state, and synthetic movie details. A contact-sheet review found no private paths, credentials, real library content, or accidental system dialogs.
