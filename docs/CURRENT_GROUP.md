# Current group

## Group 5 - Data, reliability, and release preparation

Status: complete

Completed:

- Reviewed the established SQLite format and retained its integer identifiers and compatible tables.
- Added schema version 1 and repair of tables missing from an interrupted first launch; no destructive migration was required.
- Replaced executable-configuration rewriting with an in-memory profile connection path.
- Added consistent database/image backups with manifests and SHA-256 file verification.
- Added automatic daily backups, manual backup/restore, pre-restore safety backups, staged replacement, and rollback.
- Added corrupt-database preservation and automatic recovery from the newest valid local backup.
- Added path-redacted JSON catalog export and duplicate/missing file/directory health checks.
- Added bounded archive validation and rejection of malformed or unsafe backups.
- Added rolling local logging and application-level exception containment.
- Added a generated synthetic `--demo` profile with five neutral library items and original covers.
- Added a repeatable Release x64 portable-package script and verified its personal-data exclusions.
- Added automated backup/restore, recovery, export, demo, invalid-backup, and 2,500-record scan coverage.
- Kept the real user database byte-for-byte unchanged during all isolated verification.

Next group:

- Group 6 modernizes the application shell and all primary interaction surfaces, then captures exact before/after evidence.

Remaining roadmap:

- Group 7 completes packaging and portfolio closure.
