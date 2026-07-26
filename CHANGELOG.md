# Changelog

## Unreleased

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
