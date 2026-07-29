# Testing and verification

Media Manager combines automated regression checks, isolated application walkthroughs, matched visual evidence, and release-package inspection.

## Automated stability executable

Build `MediaManager.sln` for x64, then run:

```powershell
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

The executable creates its own randomly named temp profile and removes it after the run. It verifies:

- TV-show/season/episode ownership and destructive deletion boundaries;
- harmless null selection, missing artwork, missing paths, and malformed dates;
- TMDB and IGDB mapping through mocked transports;
- provider substitution, cancellation, caching, encrypted configuration, and manual fallback;
- schema-version compatibility and stable identifiers;
- duplicate/missing-path health reporting without automatic deletion;
- path-redacted catalog export;
- consistent database/image backup and restore;
- corruption recovery and malformed-backup rejection;
- automatic-backup throttling;
- generated demo isolation;
- a practical health scan with more than 2,500 records.

The tests need no real API credential, real library, or network request.

## Manual application verification

The records under `docs/manual-tests` cover the work in each group. The completed walkthrough checks:

- launch, navigation, resize, keyboard focus, and six-library empty states;
- hierarchy, cards, selection, details, sorting, filtering, and favourites;
- add/edit/remove/delete dialogs and safe cancellation;
- file/folder selection, reveal-in-Explorer, media-viewer, and game-launch failure behavior;
- provider configuration, search, manual entry, loading, offline, and error states;
- backup, restore, health check, export, and demo-profile isolation.

All manual verification uses synthetic data. A real user database is treated as read-only evidence and its SHA-256 is checked before and after release work.

## Visual evidence

The original, restored, and modern directories each contain the same 13 application surfaces. The `contrast` directory composes selected fixed captures for the README without changing the underlying evidence.

## Release verification

The complete release gate is available as one command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\verify-release.ps1
```

Both local release work and `.github/workflows/ci.yml` use this script. It performs:

1. NuGet restore from the repository configuration.
2. Debug x64 and Release x64 solution rebuilds.
3. Both stability executables passing.
4. Portable Release x64 packaging.
5. Tracked-file runtime-data, secret, and private-path scans.
6. Per-file release-manifest verification.
7. ZIP-to-stage byte verification.
8. External SHA-256 checksum verification.
9. Clean Git working-tree verification.

The historical Group 7 manual record additionally covers application/package launch smoke tests, unexpected browser-process checks, and confirmation that the real user database hash did not change. Those user-profile-sensitive checks are not run on a hosted CI machine.

The dated outcome and package checksum are maintained in [build-status.md](build-status.md) and [release.md](release.md).
