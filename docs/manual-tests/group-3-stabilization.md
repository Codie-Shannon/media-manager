# Group 3 functional verification and stabilization

Date: 2026-07-26

Result: pass

## Later verification erratum

This pass was valid within its recorded scope, but the playback coverage was too narrow. It verified that the internal video player and picture gallery opened and returned to the library; it did not verify the full-window layout or every player control. A later live review exposed the constrained player surface. The modern release corrects and evidences the issue in `v1.0.1`, and the original-interface Student Projects edition must receive the corresponding functional correction and a new complete playback pass.

## Isolation

The application ran with `MEDIA_MANAGER_DATA_DIRECTORY` pointing to a uniquely named directory under `%TEMP%`. The profile created its own SQLite database, image directories, fixture library, and database backup.

The real `%LOCALAPPDATA%\Media_Manager\MediaManagerDB.db` SHA-256 value was recorded before testing and matched after all verification. No personal library, credentials, private database, or copyrighted media entered the repository.

The disposable fixture initially contained:

- five library folders for Movies, Videos, Pictures, Music, and Games;
- one movie, two videos including an intentionally missing path, one generated picture, one silent WAV, and one harmless command launcher;
- two TV shows, each with its own season and episode, specifically to test ownership isolation;
- neutral generated cover artwork.

## Functional matrix

| Area | Verification | Result |
| --- | --- | --- |
| Libraries | Opened Movies, TV Shows, Videos, Pictures, Music, and Games | Pass |
| Hierarchy | Opened a folder and exercised Back, Forward, and breadcrumb navigation | Pass |
| Cards and details | Selected synthetic folders/items in every library and rendered the information pane | Pass |
| Add folder | Added a Videos folder through the real element selector and folder browser; verified the database row | Pass |
| Add media | Added a generated WAV through the Music file picker and save-location browser; verified the database row | Pass |
| Edit | Renamed a video through the Edit form and verified the persisted custom name | Pass |
| Remove cancel | Cancelled video and folder removal; verified records and files remained | Pass |
| Remove confirm | Confirmed removal of a disposable folder; verified its record was removed | Pass |
| Delete cancel | Cancelled video deletion; verified the database record and file remained | Pass |
| Delete confirm | Recycled a disposable video; verified both its record and source file were gone | Pass |
| Missing file | Opened an intentionally missing video; received an error dialog and the app remained responsive | Pass |
| Explorer | Revealed a synthetic video in Explorer and verified the expected fixture directory opened | Pass |
| Playback | Opened the internal video player and picture gallery and returned to the library; full-window layout was not covered | Pass within stated scope |
| Local launch | Confirmed a harmless synthetic game launcher and verified the app remained responsive | Pass |
| Legacy metadata search | Searched for movies repeatedly, closed provider windows during retrieval, and verified Media Manager stayed responsive while temporary Chrome processes were cleaned up | Pass |
| Resize | Maximized and restored the main window; the app remained responsive | Pass |
| Folder-browser cancel | Cancelled without a selection; the save location remained unset and Add stayed disabled | Pass |
| TV ownership | Removed Show A and verified only its season/episode were removed; Show B's complete branch remained | Pass |

## Defects reproduced and repaired

1. Blank card artwork crashed `ElementBase.SetImage`.
2. Default cover lookup depended on the process working directory and crashed from other launch locations.
3. Empty or malformed creation/release dates crashed details rendering.
4. TV-show details assumed a parent record always existed.
5. TV-show removal selected seasons by the show's owner rather than the show's ID, allowing sibling data removal.
6. TV-show disk deletion iterated every episode for each season and could delete the wrong branch.
7. Destructive flows could report success after cancellation or incomplete deletion.
8. Folder-browser command handling assumed a specific template source and could crash; selection state could also leak between dialogs.
9. Missing Explorer targets and game-launch errors could terminate or destabilize the application.
10. Game launch used the launcher path as its working directory rather than the game's base directory.
11. Selenium `NoSuchWindowException` escaped from an `async` raw search thread and terminated Media Manager when an IMDb or Metacritic window disappeared.

## Automated regression gate

`MediaManager.StabilityTests` creates a random temporary database, seeds two sibling TV-show branches, removes one branch through production database code, and asserts the other remains complete. It also covers null/missing artwork and invalid date formatting.

Verified commands:

```powershell
MSBuild.exe MediaManager.sln /t:Rebuild /p:Configuration=Debug /p:Platform=x64
tests\MediaManager.StabilityTests\bin\x64\Debug\MediaManager.StabilityTests.exe

MSBuild.exe MediaManager.sln /t:Rebuild /p:Configuration=Release /p:Platform=x64
tests\MediaManager.StabilityTests\bin\x64\Release\MediaManager.StabilityTests.exe
```

Both solution rebuilds completed with zero errors. Both stability runs printed `PASS: Group 3 stability tests`.

## Deferred by roadmap

- Direct-site metadata automation remains unchanged in Group 3 and moves behind provider interfaces in Group 4.
- Schema migrations, backup/export/import, and transactional consistency remain Group 5 work.
- Visual redesign and the modern screenshot group remain Group 6 work.
