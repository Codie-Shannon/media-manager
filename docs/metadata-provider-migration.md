# Metadata-provider migration

The recovered application directly automates IMDb, Metacritic, and IGDB pages with browser drivers and site-specific selectors. This behavior is retained only as restoration reference.

Group 4 will introduce:

- `IMetadataProvider`;
- provider-neutral search and detail models;
- explicit cancellation and timeouts;
- result caching;
- offline/manual entry behavior;
- provider-specific configuration outside source control.

UI, database, and filesystem workflows must not depend directly on Selenium or a provider's page structure.

