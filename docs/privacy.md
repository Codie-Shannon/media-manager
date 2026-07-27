# Privacy and release-data policy

Media Manager is local-first. It does not require an account, upload a library, host a media server, or collect analytics.

## Runtime data

Normal launch stores application state under the current Windows user's LocalAppData profile:

- SQLite library database;
- generated or user-selected managed cover images;
- DPAPI-encrypted metadata-provider configuration;
- timestamped metadata cache;
- integrity-checked backups and recovery copies;
- small rolling diagnostic logs.

The media files themselves remain in the locations selected by the user. Metadata providers receive only the searches or external identifiers needed for an explicit enrichment request.

`Media_Manager.exe --demo` creates a disposable profile under `%TEMP%\MediaManagerDemoProfile`. It seeds neutral records and generated artwork, does not read the normal profile, and can be reset by deleting that temporary directory after the application closes.

## Backup, export, and recovery boundaries

- `.mmbak` backup archives contain a consistent database snapshot, managed covers, a manifest, and SHA-256 hashes.
- Media files, provider credentials, logs, caches, machine settings, and arbitrary external paths are excluded.
- Restore validates paths, entry count, expanded size, hashes, SQLite integrity, required tables, and schema compatibility before replacement.
- Catalog export removes local filesystem roots and emits descriptive, portable data only.
- Corrupt databases are preserved locally for recovery analysis and are never committed automatically.

## Public repository gate

The tracked-file audit rejects or reviews:

- databases, journals, logs, backups, caches, keys, certificates, and local settings;
- API tokens, client secrets, and credential-bearing examples;
- absolute personal filesystem paths;
- `bin`, `obj`, `.vs`, restored packages, and portable artifacts;
- real library screenshots or copyrighted poster/cover samples.

Public screenshots and demo records use synthetic names, paths, files, and artwork. Historical recovered evidence that was not publication-safe remains outside Git.

## Portable package gate

`packaging/build-portable.ps1` stages only Release runtime files plus public documentation. It fails if it finds database, provider-setting, log, backup, recovery, cache, Git, or debug-symbol artifacts. The final ZIP receives a separate SHA-256 file.

The Group 7 audit also scans the tracked repository and staged package for private paths and common secret formats. Verification results are recorded in [build-status.md](build-status.md) and [release.md](release.md).
