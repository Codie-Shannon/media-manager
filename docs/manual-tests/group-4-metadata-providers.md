# Group 4 metadata-provider verification

Date: 2026-07-27

All application checks used a disposable profile selected through `MEDIA_MANAGER_DATA_DIRECTORY`. The real user database SHA-256 remained `885446BEF1FDCB43678DE1D9542C3519F516AD0EB8E5B1055F5725D54957DB85`.

## Matrix

| Area | Verification | Result |
| --- | --- | --- |
| Provider boundary | Search/detail code consumes `IMetadataProvider` and neutral models | Pass |
| TMDB | Synthetic movie search/details include title, artwork URL, credits, genres, rating, and certification | Pass |
| IGDB | Synthetic game search/details include title, artwork URL, publisher, ratings, genres, and platforms | Pass |
| Live IGDB smoke | Existing local credentials authenticated and returned a real three-item query without being printed or committed | Pass |
| Cancellation | A cancelled provider request raises `OperationCanceledException` and is contained | Pass |
| Timeouts/errors | Service applies a 12-second timeout and converts auth/rate/network failures to non-fatal provider failures | Pass |
| Cache | Provider-neutral payload round-trips with provider and UTC retrieval metadata | Pass |
| Credentials | Synthetic credentials are absent from the plaintext local settings file | Pass |
| Manual/offline | With no credentials, search returns a selectable manual result and details preserve the entered title | Pass |
| Legacy IMDb | Existing IMDb title IDs route through TMDB external-ID lookup; no IMDb page is opened | Pass |
| Browser removal | Source/runtime contain no Selenium, WebDriver, UndetectedChrome, or AngleSharp application artifacts | Pass |
| Settings | Provider status, secret inputs, encrypted-storage notice, and attribution render in the recovered Settings panel | Pass |
| Isolated launch | Debug and Release applications remain alive/responding and start no new Chrome process | Pass |

## Automated gate

After an x64 build:

```powershell
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

Expected output:

```text
PASS: Group 3 and Group 4 stability tests
```
