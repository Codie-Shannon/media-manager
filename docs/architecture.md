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

