# Release packaging

Media Manager v1.0.1 is published as a repeatable portable Release x64 ZIP:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File packaging\build-portable.ps1
```

The script:

1. Rebuilds the complete solution as Release x64.
2. Stages the application runtime under `artifacts/MediaManager-portable-x64`.
3. Removes PDB/XML development output.
4. Adds portable instructions, version, README, changelog, release notes, and third-party notices.
5. Rejects database, provider-setting, log, backup, recovery, cache, key, certificate, private-path, or `.git` artifacts.
6. Creates a per-file SHA-256 release manifest.
7. Copies `RELEASE-MANIFEST.txt` beside the package for CI and release verification.
8. Creates `MediaManager-portable-x64.zip` and a separate SHA-256 checksum file.

Normal launch creates a per-user LocalAppData profile. `Media_Manager.exe --demo` creates a disposable synthetic temp profile.

The v1.0.1 release format is a portable ZIP. It is not code-signed and does not include an installer. Run `tools\verify-release.ps1` for the complete release gate. Rebuild and verification details are recorded in `docs/release.md` and `docs/build-status.md`.
