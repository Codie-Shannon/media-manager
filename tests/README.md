# Tests

## Restoration and release stability tests

`MediaManager.StabilityTests` is a dependency-light executable included in `MediaManager.sln`. It creates and removes its own randomly named database under the system temp directory.

It currently verifies:

- removing one TV show removes only that show's seasons and episodes;
- a sibling show with the same library owner remains intact;
- a null TV-show selection is harmless;
- missing artwork and null, empty, or invalid dates do not crash formatting.
- TMDB movie search/detail responses map into provider-neutral models;
- IGDB game search/detail responses map into provider-neutral models;
- provider requests honor cancellation;
- metadata cache payloads round-trip from a disposable profile;
- local provider credentials are encrypted rather than written as plaintext;
- movie search provides a manual result with no provider configuration.
- schema version 1 retains established identifiers without a destructive migration;
- missing and duplicate library paths are reported without deleting records;
- catalog export redacts local filesystem roots;
- a consistent database/image backup restores mutated records and a removed managed cover;
- a deliberately corrupted database recovers from the newest automatic backup;
- malformed backup input is rejected;
- automatic backups are throttled;
- the generated demo profile contains five available synthetic paths;
- more than 2,500 records complete the practical health-scan bound.

After building x64, run:

```powershell
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

Expected output:

```text
PASS: Group 3, Group 4, and Group 5 stability tests
```

The executable retains its historical success string for compatibility. Group 6 and Group 7 add visual, package, privacy, and clean-release verification around this automated core; see `docs/testing.md`.
