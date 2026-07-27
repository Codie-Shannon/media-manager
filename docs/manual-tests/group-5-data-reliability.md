# Group 5 data reliability and Release preparation

Date: 2026-07-27

All destructive and generated-data checks used disposable temp profiles. The real user database SHA-256 remained `885446BEF1FDCB43678DE1D9542C3519F516AD0EB8E5B1055F5725D54957DB85`.

## Matrix

| Area | Verification | Result |
| --- | --- | --- |
| Data format | Existing integer primary keys retained; schema version set to 1 without column migration | Pass |
| Protected-location readiness | Startup no longer rewrites executable configuration | Pass |
| Backup | Consistent SQLite snapshot, managed images, manifest, and SHA-256 records created | Pass |
| Restore | Mutated rows and removed managed cover restored from verified backup | Pass |
| Restore safety | Pre-restore safety backup and staged same-volume rollback present | Pass |
| Invalid backup | Non-archive and unverified contents rejected without replacing active data | Pass |
| Corruption recovery | Deliberately corrupted disposable database recovered from newest verified automatic backup | Pass |
| Health check | One missing path and one duplicate path detected without deleting records | Pass |
| Large library | More than 2,500 records scanned within the 30-second practical test bound | Pass |
| Catalog export | Local root absent and `sample://` path references present | Pass |
| Demo | Packaged `--demo` launch shows generated Sample Horizon cover and five neutral records | Pass |
| Automatic backup | Fresh demo launch creates one throttled automatic backup | Pass |
| Portable package | Release x64 folder/ZIP contains no database, credentials, logs, cache, recovery, or personal data | Pass |
| Isolation | Demo launch starts no Chrome process and real database hash is unchanged | Pass |

## Automated gate

```powershell
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

Expected:

```text
PASS: Group 3, Group 4, and Group 5 stability tests
```

## Repeatable package

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File packaging\build-portable.ps1
```

The generated Release candidate is ignored by Git under `artifacts/`.
