# Recovered architecture

## Projects

### Media_Manager

The full .NET Framework 4.7.2 WPF application. It contains navigation for Movies, TV Shows, Videos, Pictures, Music, and Games; SQLite/Dapper persistence; local file operations; playback/launching; cover handling; and legacy metadata automation.

### MediaControlsLibrary

A substantial reusable WPF control library containing navigation, submenu, forms, folder browsing, media cards, details panes, viewers, dialogs, converters, and theme resources.

### MediaControlsTester

A demonstration harness for the controls library. Some navigation labels are intentionally present only to demonstrate styling and were never configured as destinations.

## Current design characteristics

- MVVM namespaces exist, but most behavior remains in view code-behind.
- Several large views duplicate workflow and UI-state logic.
- Global/static helpers coordinate persistence, formatting, metadata, and application state.
- SQLite schema creation exists, but schema versioning and migrations do not.
- Filesystem changes and database changes are not transactionally coordinated.

The restoration plan first makes this architecture reproducible and safe, then separates responsibilities incrementally without rewriting the product.

## Metadata providers

Group 4 adds a provider boundary inside `Media_Manager.Metadata`:

- `IMetadataProvider` defines supported media kinds, search, and detail retrieval.
- `MetadataService` selects TMDB for film/TV metadata and IGDB for game metadata without exposing provider response types to the UI.
- `TmdbMetadataProvider` also resolves legacy IMDb title IDs through TMDB's supported external-ID endpoint.
- `MetadataCache` stores provider, retrieval time, and provider-neutral payload under the active local-data profile.
- `ProviderSettingsStore` protects credentials with Windows DPAPI for the current user; environment variables override local settings.
- `Fetcher` retains local MediaInfo/filesystem extraction but consumes only provider-neutral `MediaMetadata`.
- `SearchEngine` owns cancellation and maps neutral results into the recovered search control until its visual/API naming is modernized in Group 6.

Legacy database columns and CLR properties named after IMDb, Metacritic, or IGDB remain for schema compatibility. They hold external/provider references only; no page is scraped. Schema naming and migration belong to Group 5.
