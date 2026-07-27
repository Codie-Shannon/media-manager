# Data recovery and library maintenance

## Local profile

Normal operation stores user-owned state under:

```text
%LOCALAPPDATA%\Media_Manager
```

`MEDIA_MANAGER_DATA_DIRECTORY` can select an isolated profile for testing. `Media_Manager.exe --demo` selects `%TEMP%\MediaManagerDemoProfile` and creates only generated sample files.

## Automatic backups

After a healthy startup, Media Manager creates at most one automatic backup per 24 hours under the profile `Backups` directory and retains the newest seven automatic snapshots.

Each `.mmbak` contains:

- a consistent SQLite snapshot;
- managed cover/preview images;
- a format/schema manifest;
- size and SHA-256 metadata for every included file.

It excludes:

- original movie, episode, music, picture, video, and game files;
- TMDB/IGDB credentials;
- logs, metadata cache, browse settings, and other machine-local configuration.

## Manual backup and restore

Open **Settings** and choose:

- **Back Up Library** to create a user-selected `.mmbak`;
- **Restore Library Backup** to validate and restore one.

Restore performs all validation before replacing data, creates a safety backup, stages the replacement, and rolls back an interrupted failure. After success the app closes so the restored database can be loaded from a clean process.

Never rename arbitrary ZIP files to `.mmbak`. Invalid manifests, path traversal, excessive archives, hash failures, database corruption, missing tables, and newer unsupported schema versions are rejected.

## Startup recovery

If SQLite integrity checking fails:

1. The damaged database is preserved under `Recovery`.
2. Automatic backups are tested newest-first.
3. The first fully valid backup is restored.
4. Startup reports that recovery occurred.

If no valid backup exists, startup stops with the preserved recovery-file location instead of silently creating a blank replacement.

## Library health check

**Check Library Health** scans existing movie, season, episode, video, picture, music, and game paths. It reports:

- missing or unavailable files;
- missing or unavailable directories;
- duplicate normalized paths within the same table/field.

The scan never deletes or rewrites a library record. Missing drives therefore remain recoverable through Edit or Remove when the drive becomes available.

## Catalog export

**Export Catalog (Paths Redacted)** writes JSON and replaces filesystem paths with `sample://` references. Media titles and metadata remain in the export, so it is suitable for user-controlled interchange but should not be assumed anonymous.

Use the committed generated `sample-data/demo-catalog.json` or `--demo` mode for public evidence.
