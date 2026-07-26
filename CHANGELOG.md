# Changelog

## Unreleased

### Functional stabilization

- Added an isolated data-directory override and a publication-safe synthetic verification workflow.
- Verified all six libraries, hierarchy navigation, details, add/edit/remove/delete behavior, Explorer actions, playback surfaces, game launch, and resizing.
- Fixed TV-show ownership deletion and cross-season destructive-deletion defects.
- Fixed cancellation and deletion-result handling so database removal follows confirmed filesystem outcomes.
- Fixed folder-browser selection, stale state, duplicate registration, and close cleanup.
- Fixed crashes caused by blank artwork, missing default covers, invalid dates, missing TV-show parents, and folder selection.
- Made missing Explorer paths and game-launch failures non-fatal.
- Added the x64 `MediaManager.StabilityTests` executable and passed it in Debug and Release.

### Restored application startup

- Replaced the stale controls-library DLL hint with a repository project reference.
- Made the application create its LocalAppData database directory before SQLite initialization.
- Added dependency-free managed startup error presentation.
- Verified clean tracked-file restore, Debug x64 and Release x64 builds, and launches.
- Verified fresh-profile database and image-directory creation without external setup.
- Captured the 13-image restored-state screenshot group for direct comparison with the recovered baseline.

### Recovered baseline

- Curated the authoritative application, controls library, and controls tester into one repository.
- Added original-state screenshots that contain no personal paths or copyrighted sample libraries.
- Added and verified the silent recovered-state application walkthrough.
- Completed the controls, styles, templates, resources, assets, and application-view inventory.
- Replaced recovered tester sample artwork with neutral synthetic assets in the public repository copy.
- Documented the known build, startup, persistence, metadata, and destructive-workflow risks.
