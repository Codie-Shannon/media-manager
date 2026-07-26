# Current group

## Group 4 - Introduce metadata-provider architecture

Status: complete

Completed:

- Added `IMetadataProvider` and provider-neutral search/detail models.
- Added TMDB for movies, TV shows, seasons, and episodes.
- Added IGDB for games and verified the existing local credentials against the live supported API without committing them.
- Replaced both popup search and Add/Edit detail scraping with provider calls.
- Added cancellation, timeouts, timestamped search/detail caching, stale-cache fallback, and friendly provider errors.
- Added manual search results so local library workflows remain available with no key or no network.
- Added local DPAPI-encrypted provider settings and environment-variable overrides.
- Added provider attribution to the Settings surface.
- Removed Selenium, Chrome-driver, WebDriverManager, AngleSharp, and all active direct-site selectors from the application project.
- Added mocked provider, cancellation, caching, encrypted-settings, and offline/manual regression coverage.
- Preserved the original user database byte-for-byte during isolated verification.

Next group:

- Group 5 improves local-data reliability, backup/export/import behavior, identifiers, and release preparation.

Remaining roadmap:

- Group 6 performs the modern UI redesign and captures the final screenshot group.
- Group 7 completes packaging and portfolio closure.
