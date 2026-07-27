# Synthetic sample data

Only generated, non-copyrighted fixtures may be added here. Fixtures use neutral names and paths and never reference a personal media library.

Group 3 used a disposable synthetic library generated under the system temp directory. It included tiny neutral video placeholders, a generated PNG, a silent WAV, a harmless command launcher, two TV-show ownership branches, and one intentionally missing path. The private test directory and database are not repository artifacts.

Group 5 adds `demo-catalog.json` and a working application demo mode:

```powershell
src\Media_Manager\bin\x64\Release\Media_Manager.exe --demo
```

Demo mode creates `%TEMP%\MediaManagerDemoProfile` with generated cover images, tiny neutral placeholder files, five library records, a local log, and an automatic backup. It never reads the normal user database. Delete that disposable temp directory to reset the demo.

The automated stability suite creates and removes its own temporary records, backup archives, corrupt database, generated images, and 2,500-record scan library.
