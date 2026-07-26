# Current group

## Group 3 - Verify and stabilize existing functionality

Status: complete

Completed:

- Added an opt-in data-directory override so verification can run in a disposable profile without touching the user's real database.
- Exercised Movies, TV Shows, Videos, Pictures, Music, and Games using neutral synthetic folders and media.
- Verified hierarchy navigation, Back/Forward, selection/details, favourites surfaces, add, edit, remove, delete, cancel, Explorer reveal, video/picture playback surfaces, game launch, and resize behavior.
- Fixed null and missing cover crashes, invalid/empty date crashes, and a null TV-show parent lookup.
- Fixed TV-show ownership deletion so removing one show cannot remove a sibling show's seasons or episodes.
- Made destructive workflows preflight paths and untracked files, preserve cancellation, and report success only after deletion succeeds.
- Fixed folder-browser selection, duplicate registration, stale selection, and close cleanup.
- Replaced unsafe metadata-search thread abortion with serialized, exception-contained driver cleanup so provider-window failures remain non-fatal.
- Made missing Explorer targets and game-launch failures non-fatal.
- Added and passed `MediaManager.StabilityTests` in Debug x64 and Release x64.
- Preserved the original user database byte-for-byte during isolated verification.

Next group:

- Group 4 replaces direct-site metadata scraping with a supported, swappable metadata-provider architecture. It does not redesign the UI.

Remaining roadmap:

- Group 5 improves data reliability and prepares the application for release.
- Group 6 performs the modern UI redesign and captures the final screenshot group.
- Group 7 completes packaging and portfolio closure.
