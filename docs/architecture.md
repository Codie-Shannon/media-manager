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
- SQLite schema version 1 records the reviewed compatible format; existing primary keys remain stable.
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

Legacy database columns and CLR properties named after IMDb, Metacritic, or IGDB remain for schema compatibility. They hold external/provider references only; no page is scraped. Group 5 reviewed these names and retained them to avoid a risky, low-value data migration.

## Data reliability

Group 5 adds a bounded recovery layer inside `Media_Manager.Data`:

- `LibraryDataService` owns consistent SQLite snapshots, managed-image backup, restore validation, staged replacement, rollback, corruption recovery, path-redacted catalog export, health checks, and demo seeding.
- `.mmbak` files are ZIP containers with a manifest and SHA-256 record for every included database/image file.
- Provider credentials, logs, metadata cache, media files, and machine settings are deliberately excluded from backups.
- Restore validates archive paths, size, entry count, hashes, database integrity, required tables, and schema compatibility before touching active data.
- A pre-restore backup and same-volume rollback directory make a failed replacement recoverable.
- `ApplicationLog` writes a small rolling local log while never allowing logging failure to terminate the app.
- `Database` keeps its connection path in process memory rather than modifying `Media_Manager.exe.config`, which makes protected-location portable/installer execution viable.
- Existing `Id INTEGER PRIMARY KEY` values and legacy compatibility columns are retained. A risky identifier or column migration was not justified.
- The original view-level duplicate checks remain; the new library-wide health scan finds duplicates and unavailable paths without deleting records automatically.

Filesystem delete workflows remain intentionally separate from database backup/restore. Broader transactional coordination is deferred unless real testing demonstrates a blocking defect.
