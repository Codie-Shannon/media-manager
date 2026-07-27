# Metadata-provider architecture

Group 4 removes the recovered application's browser-driven IMDb, Metacritic, and IGDB retrieval paths.

## Providers

| Media | Active provider | Supported operations |
| --- | --- | --- |
| Movies | TMDB | Search, details, credits, certification, artwork |
| TV shows | TMDB | Search, details, credits, content rating, season/episode counts |
| Seasons | TMDB | Details, episode count, artwork |
| Episodes | TMDB | Details, credits, air date, artwork |
| Games | IGDB | Search, details, publisher, ratings, genres, platforms, artwork |

The UI and local fetcher depend on `IMetadataProvider` and provider-neutral models. Provider-specific JSON and authentication remain inside the provider classes.

## Credential setup

Open **Settings > Configure Metadata Providers**.

- TMDB requires an API Read Access Token.
- IGDB requires a Twitch Developer application Client ID and Client Secret.
- Leaving a secret field blank preserves the saved value.

Saved values are protected with Windows DPAPI for the current user and written under the active Media Manager data profile as `metadata-providers.json`. The file contains encrypted values, not plaintext credentials.

Environment variables override saved values:

```text
MEDIA_MANAGER_TMDB_ACCESS_TOKEN
MEDIA_MANAGER_IGDB_CLIENT_ID
MEDIA_MANAGER_IGDB_CLIENT_SECRET
```

Do not put real values in source-controlled files, command examples, screenshots, or issue reports.

## Failure and offline behavior

- Every text search includes a manual result.
- A missing credential never prevents local library use.
- Provider calls are cancellable and limited to 12 seconds.
- Authentication, network, timeout, and rate-limit failures do not start a browser or terminate the application.
- Fresh cache entries are used before the network.
- Expired cache entries may be used when a provider fails.
- Search cache lifetime is 24 hours; detail cache lifetime is seven days.
- Cache envelopes record provider and UTC retrieval time.

## Legacy compatibility

The Group 4 implementation intentionally does not migrate the SQLite schema. Existing property/column names such as `IMDBLink` and `MetaCriticLink` remain compatibility fields until Group 5.

Existing IMDb title references are resolved through TMDB's supported `/find` external-ID workflow when TMDB is configured. No IMDb or Metacritic page is opened, parsed, or automated.

## Attribution

The Settings surface includes the required notice:

> This product uses the TMDB API but is not endorsed or certified by TMDB.

Game metadata is identified as supplied by IGDB. Group 7 reviewed provider attribution and deliberately redistributes no provider logos.

Provider references:

- [TMDB application authentication](https://developer.themoviedb.org/docs/authentication-application)
- [TMDB search and details workflow](https://developer.themoviedb.org/docs/search-and-query-for-details)
- [TMDB attribution requirements](https://developer.themoviedb.org/docs/faq)
- [IGDB authentication, requests, and rate limits](https://api-docs.igdb.com/)

## Verification

`MediaManager.StabilityTests` uses synthetic HTTP responses to verify TMDB and IGDB parsing, provider substitution, cancellation, caching, encrypted settings, and manual fallback. The test suite does not require network access or real credentials.
