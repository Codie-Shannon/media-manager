# Packaging

Group 5 provides a repeatable portable Release candidate:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File packaging\build-portable.ps1
```

The script:

1. Rebuilds the complete solution as Release x64.
2. Stages the application runtime under `artifacts/MediaManager-portable-x64`.
3. Removes PDB/XML development output.
4. Adds portable run instructions, README, and changelog.
5. Rejects database, provider-setting, log, backup, recovery, cache, or `.git` artifacts.
6. Creates `MediaManager-portable-x64.zip` and a SHA-256 checksum file.

Normal launch creates a per-user LocalAppData profile. `Media_Manager.exe --demo` creates a disposable synthetic temp profile.

This is a draft portable process for repeatability testing. Group 7 will decide the final portable/installer format, provenance, signing, and clean-machine distribution evidence.
