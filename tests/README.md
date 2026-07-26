# Tests

## Group 3 and Group 4 stability tests

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

After building x64, run:

```powershell
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

Future suites will cover persistence migrations and additional synthetic integration workflows.
