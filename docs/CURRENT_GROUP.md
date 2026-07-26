# Current group

## Group 2 - Restore reproducible build and startup

Status: complete

Completed:

- Replaced the broken controls DLL hint with a project reference.
- Made database initialization create its LocalAppData directory before opening SQLite.
- Removed implicit `StartupUri` construction and added controlled managed startup.
- Added a dependency-free WPF startup error window.
- Verified the complete solution in Debug x64 and Release x64.
- Verified restore, both builds, and both launches from a fresh tracked-file checkout.
- Verified fresh-profile creation of the database and all required image directories.
- Preserved the original user profile after isolated startup testing.
- Captured and visually audited all 13 restored-state screenshots.

Next group:

- Group 3 verifies and stabilizes the existing functionality from this working, documented restoration checkpoint. It does not redesign the UI.

Remaining roadmap:

- Group 4 replaces direct IMDb scraping with a supported, swappable metadata-provider architecture.
- Group 5 improves data reliability and prepares the application for release.
- Group 6 performs the modern UI redesign and captures the final screenshot group.
- Group 7 completes packaging and portfolio closure.
