using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Media_Manager.Data
{
    public static class LibraryDataService
    {
        private const int BackupFormatVersion = 1;
        private const long MaximumRestoreBytes = 20L * 1024 * 1024 * 1024;
        private const int MaximumRestoreEntries = 100000;
        private static readonly string[] RequiredTables =
        {
            "Folders",
            "Movies",
            "TVShowFolders",
            "SeasonFolders",
            "Episodes",
            "Videos",
            "Pictures",
            "Music",
            "Games"
        };

        private static readonly PathDescriptor[] PathDescriptors =
        {
            new PathDescriptor("Movies", "FilePath", false),
            new PathDescriptor("SeasonFolders", "FilePath", true),
            new PathDescriptor("Episodes", "FilePath", false),
            new PathDescriptor("Videos", "FilePath", false),
            new PathDescriptor("Pictures", "FilePath", false),
            new PathDescriptor("Music", "FilePath", false),
            new PathDescriptor("Games", "FilePath", false),
            new PathDescriptor("Games", "BaseDirectory", true)
        };

        private static readonly HashSet<string> PathColumns =
            new HashSet<string>(
                new[]
                {
                    "FilePath",
                    "BaseDirectory",
                    "CoverImage",
                    "CustomCoverImage"
                },
                StringComparer.OrdinalIgnoreCase);

        private static string dataDirectory;
        private static string databasePath;
        private static string imagesDirectory;
        private static string backupsDirectory;
        private static string recoveryDirectory;
        private static string temporaryDirectory;

        public static string DataDirectory => dataDirectory;
        public static string DatabasePath => databasePath;
        public static string BackupsDirectory => backupsDirectory;

        public static void Initialize(string localDataDirectory)
        {
            if (string.IsNullOrWhiteSpace(localDataDirectory))
            {
                throw new ArgumentException(
                    "A local data directory is required.",
                    nameof(localDataDirectory));
            }

            dataDirectory = Path.GetFullPath(localDataDirectory);
            databasePath = Path.Combine(dataDirectory, "MediaManagerDB.db");
            imagesDirectory = Path.Combine(dataDirectory, "Images");
            backupsDirectory = Path.Combine(dataDirectory, "Backups");
            recoveryDirectory = Path.Combine(dataDirectory, "Recovery");
            temporaryDirectory = Path.Combine(dataDirectory, "Temp");

            Directory.CreateDirectory(dataDirectory);
            Directory.CreateDirectory(imagesDirectory);
            Directory.CreateDirectory(backupsDirectory);
            Directory.CreateDirectory(recoveryDirectory);
            Directory.CreateDirectory(temporaryDirectory);
        }

        public static bool RecoverDatabaseIfRequired()
        {
            EnsureInitialized();
            if (!File.Exists(databasePath)
                || IsDatabaseHealthy(databasePath, false))
            {
                return false;
            }

            string corruptPath = Path.Combine(
                recoveryDirectory,
                $"MediaManagerDB-corrupt-{DateTime.UtcNow:yyyyMMdd-HHmmss}.db");
            File.Move(databasePath, corruptPath);
            ApplicationLog.Warning(
                "Database integrity check failed; attempting automatic recovery.");

            foreach (string backup in Directory
                .GetFiles(backupsDirectory, "*.mmbak")
                .OrderByDescending(File.GetLastWriteTimeUtc))
            {
                try
                {
                    RestoreBackupCore(backup, false);
                    ApplicationLog.Info(
                        "Database recovered from the newest valid automatic backup.");
                    return true;
                }
                catch (Exception exception)
                {
                    ApplicationLog.Error(
                        "An automatic backup could not be used for recovery.",
                        exception);
                }
            }

            if (!File.Exists(databasePath))
            {
                File.Copy(corruptPath, databasePath, true);
            }

            throw new LibraryDataException(
                "The library database is damaged and no valid backup is available. "
                + $"A recovery copy was preserved at {corruptPath}.");
        }

        public static string CreateAutomaticBackupIfDue()
        {
            EnsureInitialized();
            DateTime cutoff = DateTime.UtcNow.AddHours(-24);
            string newest = Directory
                .GetFiles(backupsDirectory, "MediaManager-auto-*.mmbak")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
            if (newest != null && File.GetLastWriteTimeUtc(newest) >= cutoff)
            {
                return newest;
            }

            string destination = Path.Combine(
                backupsDirectory,
                $"MediaManager-auto-{DateTime.UtcNow:yyyyMMdd-HHmmss}.mmbak");
            CreateBackup(destination);

            foreach (string oldBackup in Directory
                .GetFiles(backupsDirectory, "MediaManager-auto-*.mmbak")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Skip(7))
            {
                TryDeleteFile(oldBackup);
            }

            return destination;
        }

        public static void CreateBackup(string destinationPath)
        {
            EnsureInitialized();
            if (!File.Exists(databasePath))
            {
                throw new LibraryDataException(
                    "The library database does not exist yet.");
            }

            string destination = NormalizeDestination(
                destinationPath,
                ".mmbak");
            string staging = CreateTemporaryDirectory("backup");
            string temporaryBackup = destination + ".tmp";
            try
            {
                string stagedDatabase = Path.Combine(
                    staging,
                    "MediaManagerDB.db");
                CreateConsistentDatabaseCopy(stagedDatabase);
                if (!IsDatabaseHealthy(stagedDatabase, true))
                {
                    throw new LibraryDataException(
                        "The database snapshot failed its integrity check.");
                }

                if (Directory.Exists(imagesDirectory))
                {
                    CopyDirectory(
                        imagesDirectory,
                        Path.Combine(staging, "Images"));
                }

                BackupManifest manifest = new BackupManifest
                {
                    FormatVersion = BackupFormatVersion,
                    DatabaseSchemaVersion = Database.SchemaVersion,
                    CreatedAtUtc = DateTime.UtcNow,
                    ApplicationVersion =
                        Assembly.GetExecutingAssembly().GetName().Version.ToString()
                };
                foreach (string file in Directory
                    .GetFiles(staging, "*", SearchOption.AllDirectories)
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
                {
                    manifest.Files.Add(new BackupFileRecord
                    {
                        RelativePath = RelativePath(staging, file),
                        Length = new FileInfo(file).Length,
                        Sha256 = HashFile(file)
                    });
                }

                File.WriteAllText(
                    Path.Combine(staging, "manifest.json"),
                    JsonConvert.SerializeObject(manifest, Formatting.Indented),
                    Encoding.UTF8);

                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                TryDeleteFile(temporaryBackup);
                ZipFile.CreateFromDirectory(
                    staging,
                    temporaryBackup,
                    CompressionLevel.Optimal,
                    false);
                ReplaceFile(temporaryBackup, destination);
                ApplicationLog.Info("Library backup completed.");
            }
            catch (Exception exception) when (!(exception is LibraryDataException))
            {
                throw new LibraryDataException(
                    "Media Manager could not create the library backup.",
                    exception);
            }
            finally
            {
                TryDeleteFile(temporaryBackup);
                TryDeleteDirectory(staging);
            }
        }

        public static void RestoreBackup(string backupPath)
        {
            EnsureInitialized();
            RestoreBackupCore(backupPath, true);
            ApplicationLog.Info("Library backup restored successfully.");
        }

        public static LibraryHealthReport CheckLibrary(
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            LibraryHealthReport report = new LibraryHealthReport
            {
                CheckedAtUtc = DateTime.UtcNow
            };

            using (SQLiteConnection connection = OpenReadOnly(databasePath))
            {
                foreach (string table in RequiredTables)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using (SQLiteCommand count = new SQLiteCommand(
                        $"select count(*) from [{table}]",
                        connection))
                    {
                        report.TotalRecords += Convert.ToInt32(
                            count.ExecuteScalar());
                    }
                }

                foreach (PathDescriptor descriptor in PathDescriptors)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Dictionary<string, List<int>> matchingPaths =
                        new Dictionary<string, List<int>>(
                            StringComparer.OrdinalIgnoreCase);
                    using (SQLiteCommand command = new SQLiteCommand(
                        $"select Id, [{descriptor.Column}] from [{descriptor.Table}]",
                        connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            string value = reader.IsDBNull(1)
                                ? string.Empty
                                : reader.GetString(1);
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                continue;
                            }

                            int id = Convert.ToInt32(reader["Id"]);
                            report.CheckedPaths++;
                            string key = NormalizeComparisonPath(value);
                            List<int> ids;
                            if (!matchingPaths.TryGetValue(key, out ids))
                            {
                                ids = new List<int>();
                                matchingPaths[key] = ids;
                            }

                            ids.Add(id);
                            if (!PathExists(value, descriptor.IsDirectory))
                            {
                                report.MissingPaths++;
                                report.Issues.Add(new LibraryHealthIssue
                                {
                                    Table = descriptor.Table,
                                    Id = id,
                                    Kind = descriptor.IsDirectory
                                        ? "Missing directory"
                                        : "Missing file",
                                    Path = value
                                });
                            }
                        }
                    }

                    foreach (KeyValuePair<string, List<int>> duplicate
                        in matchingPaths.Where(item => item.Value.Count > 1))
                    {
                        report.DuplicatePaths += duplicate.Value.Count - 1;
                        foreach (int id in duplicate.Value.Skip(1))
                        {
                            report.Issues.Add(new LibraryHealthIssue
                            {
                                Table = descriptor.Table,
                                Id = id,
                                Kind = "Duplicate path",
                                Path = duplicate.Key
                            });
                        }
                    }
                }
            }

            ApplicationLog.Info(
                "Library health check completed. " + report.Summary);
            return report;
        }

        public static void ExportCatalog(
            string destinationPath,
            bool includePersonalPaths = false)
        {
            EnsureInitialized();
            string destination = NormalizeDestination(
                destinationPath,
                ".json");
            JObject catalog = new JObject
            {
                ["format"] = "Media Manager catalog",
                ["formatVersion"] = 1,
                ["exportedAtUtc"] = DateTime.UtcNow,
                ["pathsRedacted"] = !includePersonalPaths,
                ["tables"] = new JObject()
            };
            JObject tables = (JObject)catalog["tables"];

            using (SQLiteConnection connection = OpenReadOnly(databasePath))
            {
                foreach (string table in RequiredTables)
                {
                    JArray rows = new JArray();
                    using (SQLiteCommand command = new SQLiteCommand(
                        $"select * from [{table}] order by Id",
                        connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            JObject row = new JObject();
                            for (int index = 0; index < reader.FieldCount; index++)
                            {
                                string name = reader.GetName(index);
                                object value = reader.IsDBNull(index)
                                    ? null
                                    : reader.GetValue(index);
                                if (!includePersonalPaths
                                    && PathColumns.Contains(name)
                                    && value != null)
                                {
                                    value = RedactPath($"{value}", table);
                                }

                                row[name] = value == null
                                    ? JValue.CreateNull()
                                    : JToken.FromObject(value);
                            }

                            rows.Add(row);
                        }
                    }

                    tables[table] = rows;
                }
            }

            string temporary = destination + ".tmp";
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.WriteAllText(
                    temporary,
                    catalog.ToString(Formatting.Indented),
                    Encoding.UTF8);
                ReplaceFile(temporary, destination);
                ApplicationLog.Info("Privacy-safe library catalog exported.");
            }
            finally
            {
                TryDeleteFile(temporary);
            }
        }

        public static void EnsureDemoLibrary()
        {
            EnsureInitialized();
            if (!File.Exists(databasePath))
            {
                throw new LibraryDataException(
                    "Initialize the demo database before adding sample data.");
            }

            string demoMedia = Path.Combine(dataDirectory, "DemoMedia");
            string demoImages = Path.Combine(imagesDirectory, "Demo Covers");
            Directory.CreateDirectory(demoMedia);
            Directory.CreateDirectory(demoImages);

            string movieFile = CreatePlaceholderFile(
                demoMedia,
                "sample-horizon.mp4");
            string videoFile = CreatePlaceholderFile(
                demoMedia,
                "sample-reel.mp4");
            string pictureFile = Path.Combine(
                demoMedia,
                "sample-frame.png");
            string songFile = CreatePlaceholderFile(
                demoMedia,
                "sample-tone.wav");
            string gameDirectory = Path.Combine(
                demoMedia,
                "Sample Quest");
            Directory.CreateDirectory(gameDirectory);
            string gameFile = CreatePlaceholderFile(
                gameDirectory,
                "sample-quest.cmd",
                "@echo Sample Quest demo launcher");

            string movieCover = Path.Combine(
                demoImages,
                "sample-horizon.png");
            string videoCover = Path.Combine(
                demoImages,
                "sample-reel.png");
            string gameCover = Path.Combine(
                demoImages,
                "sample-quest.png");
            CreateDemoImage(movieCover, "SAMPLE", "HORIZON");
            CreateDemoImage(videoCover, "SAMPLE", "REEL");
            CreateDemoImage(gameCover, "SAMPLE", "QUEST");
            CreateDemoImage(pictureFile, "SAMPLE", "FRAME");

            using (SQLiteConnection connection = new SQLiteConnection(
                $"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                if (TableCount(connection, "Movies") == 0)
                {
                    ExecuteDemoInsert(
                        connection,
                        @"insert into Movies
                        (OwnerId, isFavourite, FilePath, CoverImage, Name,
                         Width, Height, Duration, Framerate, Format, FileSize,
                         CreationTime, CreationDate, ReleaseDate, Region,
                         AgeRating, SerializedGenres, SerializedDirectors,
                         SerializedWriters, SerializedStars,
                         SerializedProductionCompanies)
                        values
                        (0, 1, @file, @cover, 'Sample Horizon',
                         '1920', '1080', 600000, 24, 'MP4', 1024,
                         '12:00:00', '2026-01-01', '2026-01-01', 'NZ',
                         'G', 'Adventure', 'Demo Director', 'Demo Writer',
                         'Demo Performer', 'Demo Studio')",
                        new Dictionary<string, object>
                        {
                            ["@file"] = movieFile,
                            ["@cover"] = movieCover
                        });
                }

                if (TableCount(connection, "Videos") == 0)
                {
                    ExecuteDemoInsert(
                        connection,
                        @"insert into Videos
                        (OwnerId, isFavourite, FilePath, CoverImage, Name,
                         Width, Height, Duration, Framerate, Format, FileSize,
                         CreationTime, CreationDate)
                        values
                        (0, 0, @file, @cover, 'Sample Reel',
                         '1280', '720', 90000, 30, 'MP4', 1024,
                         '12:00:00', '2026-01-01')",
                        new Dictionary<string, object>
                        {
                            ["@file"] = videoFile,
                            ["@cover"] = videoCover
                        });
                }

                if (TableCount(connection, "Pictures") == 0)
                {
                    ExecuteDemoInsert(
                        connection,
                        @"insert into Pictures
                        (OwnerId, isFavourite, FilePath, CoverImage, Name,
                         Width, Height, Format, FileSize, CreationTime,
                         CreationDate, ColourSpace, BitDepth, CompMode)
                        values
                        (0, 0, @file, @file, 'Sample Frame',
                         '600', '900', 'PNG', 1024, '12:00:00',
                         '2026-01-01', 'RGB', '32', 'Lossless')",
                        new Dictionary<string, object>
                        {
                            ["@file"] = pictureFile
                        });
                }

                if (TableCount(connection, "Music") == 0)
                {
                    ExecuteDemoInsert(
                        connection,
                        @"insert into Music
                        (OwnerId, isFavourite, FilePath, CoverImage, Name,
                         Duration, Format, FileSize, CreationTime, CreationDate,
                         SampleRate, AudioChannels, CompMode)
                        values
                        (0, 0, @file, @cover, 'Sample Tone',
                         30000, 'WAV', 1024, '12:00:00', '2026-01-01',
                         44100, 'Stereo', 'Lossless')",
                        new Dictionary<string, object>
                        {
                            ["@file"] = songFile,
                            ["@cover"] = movieCover
                        });
                }

                if (TableCount(connection, "Games") == 0)
                {
                    ExecuteDemoInsert(
                        connection,
                        @"insert into Games
                        (OwnerId, isFavourite, BaseDirectory, FilePath,
                         CoverImage, Name, Format, FileSize, CreationTime,
                         CreationDate, Publisher, ReleaseDate, Type,
                         SerializedGenres, SerializedAvailablePlatforms)
                        values
                        (0, 0, @directory, @file, @cover, 'Sample Quest',
                         'CMD', 1024, '12:00:00', '2026-01-01',
                         'Demo Studio', '2026-01-01', 'Main Game',
                         'Adventure', 'Windows')",
                        new Dictionary<string, object>
                        {
                            ["@directory"] = gameDirectory,
                            ["@file"] = gameFile,
                            ["@cover"] = gameCover
                        });
                }
            }

            ApplicationLog.Info("Synthetic demo library is ready.");
        }

        public static bool IsDatabaseHealthy(
            string path,
            bool requireSchema)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            try
            {
                using (SQLiteConnection connection = OpenReadOnly(path))
                using (SQLiteCommand command = new SQLiteCommand(
                    "PRAGMA integrity_check;",
                    connection))
                {
                    string result = $"{command.ExecuteScalar()}";
                    if (!string.Equals(
                        result,
                        "ok",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    if (!requireSchema)
                    {
                        return true;
                    }

                    foreach (string table in RequiredTables)
                    {
                        command.CommandText =
                            "select count(*) from sqlite_master "
                            + "where type='table' and name=@name";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@name", table);
                        if (Convert.ToInt32(command.ExecuteScalar()) != 1)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void RestoreBackupCore(
            string backupPath,
            bool createPreRestoreBackup)
        {
            if (string.IsNullOrWhiteSpace(backupPath)
                || !File.Exists(backupPath))
            {
                throw new LibraryDataException(
                    "The selected backup file does not exist.");
            }

            string staging = CreateTemporaryDirectory("restore");
            string rollback = Path.Combine(
                recoveryDirectory,
                "restore-rollback-" + Guid.NewGuid().ToString("N"));
            bool preserveRollback = false;
            try
            {
                ExtractBackupSafely(backupPath, staging);
                BackupManifest manifest = ReadAndValidateManifest(staging);
                string stagedDatabase = Path.Combine(
                    staging,
                    "MediaManagerDB.db");
                if (!IsDatabaseHealthy(stagedDatabase, true))
                {
                    throw new LibraryDataException(
                        "The backup database failed its integrity check.");
                }

                if (manifest.DatabaseSchemaVersion > Database.SchemaVersion)
                {
                    throw new LibraryDataException(
                        "This backup was created by a newer database format.");
                }

                if (createPreRestoreBackup && File.Exists(databasePath))
                {
                    string safetyBackup = Path.Combine(
                        backupsDirectory,
                        $"MediaManager-before-restore-{DateTime.UtcNow:yyyyMMdd-HHmmss}.mmbak");
                    CreateBackup(safetyBackup);
                }

                Directory.CreateDirectory(rollback);
                string rollbackDatabase = Path.Combine(
                    rollback,
                    "MediaManagerDB.db");
                string rollbackImages = Path.Combine(rollback, "Images");
                bool databaseMoved = false;
                bool imagesMoved = false;
                try
                {
                    if (File.Exists(databasePath))
                    {
                        File.Move(databasePath, rollbackDatabase);
                        databaseMoved = true;
                    }

                    if (Directory.Exists(imagesDirectory))
                    {
                        Directory.Move(imagesDirectory, rollbackImages);
                        imagesMoved = true;
                    }

                    File.Move(stagedDatabase, databasePath);
                    string stagedImages = Path.Combine(staging, "Images");
                    if (Directory.Exists(stagedImages))
                    {
                        Directory.Move(stagedImages, imagesDirectory);
                    }
                    else
                    {
                        Directory.CreateDirectory(imagesDirectory);
                    }
                }
                catch
                {
                    try
                    {
                        TryDeleteFile(databasePath);
                        TryDeleteDirectory(imagesDirectory);
                        if (databaseMoved && File.Exists(rollbackDatabase))
                        {
                            File.Move(rollbackDatabase, databasePath);
                        }

                        if (imagesMoved && Directory.Exists(rollbackImages))
                        {
                            Directory.Move(rollbackImages, imagesDirectory);
                        }
                    }
                    catch (Exception rollbackException)
                    {
                        preserveRollback = true;
                        ApplicationLog.Error(
                            "Restore rollback requires manual recovery from "
                            + rollback,
                            rollbackException);
                    }

                    throw;
                }
            }
            catch (LibraryDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new LibraryDataException(
                    "Media Manager could not restore the selected backup.",
                    exception);
            }
            finally
            {
                TryDeleteDirectory(staging);
                if (!preserveRollback)
                {
                    TryDeleteDirectory(rollback);
                }
            }
        }

        private static BackupManifest ReadAndValidateManifest(string staging)
        {
            string manifestPath = Path.Combine(staging, "manifest.json");
            if (!File.Exists(manifestPath))
            {
                throw new LibraryDataException(
                    "The selected file is not a Media Manager backup.");
            }

            BackupManifest manifest;
            try
            {
                manifest = JsonConvert.DeserializeObject<BackupManifest>(
                    File.ReadAllText(manifestPath, Encoding.UTF8));
            }
            catch (JsonException exception)
            {
                throw new LibraryDataException(
                    "The backup manifest is invalid.",
                    exception);
            }

            if (manifest == null
                || manifest.FormatVersion != BackupFormatVersion
                || manifest.Files == null)
            {
                throw new LibraryDataException(
                    "The backup format is not supported.");
            }

            HashSet<string> expected = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            foreach (BackupFileRecord record in manifest.Files)
            {
                string fullPath = ResolveContainedPath(
                    staging,
                    record.RelativePath);
                expected.Add(
                    record.RelativePath.Replace('\\', '/'));
                if (!File.Exists(fullPath)
                    || new FileInfo(fullPath).Length != record.Length
                    || !string.Equals(
                        HashFile(fullPath),
                        record.Sha256,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new LibraryDataException(
                        "A backup file is missing or failed verification.");
                }
            }

            string[] actual = Directory
                .GetFiles(staging, "*", SearchOption.AllDirectories)
                .Where(path => !string.Equals(
                    path,
                    manifestPath,
                    StringComparison.OrdinalIgnoreCase))
                .Select(path => RelativePath(staging, path))
                .ToArray();
            if (actual.Any(path => !expected.Contains(path))
                || actual.Length != expected.Count)
            {
                throw new LibraryDataException(
                    "The backup contains unverified files.");
            }

            return manifest;
        }

        private static void ExtractBackupSafely(
            string backupPath,
            string destination)
        {
            using (ZipArchive archive = ZipFile.OpenRead(backupPath))
            {
                if (archive.Entries.Count > MaximumRestoreEntries
                    || archive.Entries.Sum(entry => entry.Length)
                        > MaximumRestoreBytes)
                {
                    throw new LibraryDataException(
                        "The selected backup is too large to restore safely.");
                }

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string path = ResolveContainedPath(
                        destination,
                        entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(path);
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (Stream input = entry.Open())
                    using (FileStream output = new FileStream(
                        path,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }

        private static void CreateConsistentDatabaseCopy(
            string destination)
        {
            SQLiteConnection.CreateFile(destination);
            using (SQLiteConnection source = new SQLiteConnection(
                $"Data Source={databasePath};Version=3;Read Only=True;"))
            using (SQLiteConnection target = new SQLiteConnection(
                $"Data Source={destination};Version=3;"))
            {
                source.Open();
                target.Open();
                source.BackupDatabase(
                    target,
                    "main",
                    "main",
                    -1,
                    null,
                    0);
            }
        }

        private static SQLiteConnection OpenReadOnly(string path)
        {
            SQLiteConnection connection = new SQLiteConnection(
                $"Data Source={path};Version=3;Read Only=True;");
            connection.Open();
            return connection;
        }

        private static string NormalizeDestination(
            string destinationPath,
            string extension)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentException(
                    "A destination path is required.",
                    nameof(destinationPath));
            }

            string fullPath = Path.GetFullPath(destinationPath);
            if (!string.Equals(
                Path.GetExtension(fullPath),
                extension,
                StringComparison.OrdinalIgnoreCase))
            {
                fullPath += extension;
            }

            return fullPath;
        }

        private static string ResolveContainedPath(
            string root,
            string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)
                || Path.IsPathRooted(relativePath))
            {
                throw new LibraryDataException(
                    "The backup contains an unsafe path.");
            }

            string normalizedRoot = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(
                Path.Combine(
                    normalizedRoot,
                    relativePath.Replace(
                        '/',
                        Path.DirectorySeparatorChar)));
            if (!fullPath.StartsWith(
                normalizedRoot,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new LibraryDataException(
                    "The backup contains a path outside its archive.");
            }

            return fullPath;
        }

        private static string CreateTemporaryDirectory(string purpose)
        {
            string path = Path.Combine(
                temporaryDirectory,
                purpose + "-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (string directory in Directory.GetDirectories(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(
                    Path.Combine(
                        destination,
                        directory.Substring(source.Length)
                            .TrimStart(Path.DirectorySeparatorChar)));
            }

            foreach (string file in Directory.GetFiles(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                string relative = file.Substring(source.Length)
                    .TrimStart(Path.DirectorySeparatorChar);
                string target = Path.Combine(destination, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target, true);
            }
        }

        private static string RelativePath(string root, string path)
        {
            return path.Substring(
                    Path.GetFullPath(root)
                        .TrimEnd(Path.DirectorySeparatorChar)
                        .Length + 1)
                .Replace(Path.DirectorySeparatorChar, '/');
        }

        private static string HashFile(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                return BitConverter.ToString(sha.ComputeHash(stream))
                    .Replace("-", string.Empty);
            }
        }

        private static string NormalizeComparisonPath(string path)
        {
            try
            {
                return Path.GetFullPath(path)
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);
            }
            catch
            {
                return path.Trim();
            }
        }

        private static bool PathExists(string path, bool directory)
        {
            try
            {
                return directory
                    ? Directory.Exists(path)
                    : File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        private static string RedactPath(string path, string table)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            string name;
            try
            {
                name = Path.GetFileName(path.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar));
            }
            catch
            {
                name = "item";
            }

            return $"sample://{table}/{Uri.EscapeDataString(name ?? "item")}";
        }

        private static void ReplaceFile(string temporary, string destination)
        {
            if (File.Exists(destination))
            {
                File.Replace(temporary, destination, null);
            }
            else
            {
                File.Move(temporary, destination);
            }
        }

        private static string CreatePlaceholderFile(
            string directory,
            string name,
            string content = "Synthetic Media Manager demo placeholder.")
        {
            string path = Path.Combine(directory, name);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, content, Encoding.UTF8);
            }

            return path;
        }

        private static void CreateDemoImage(
            string path,
            string heading,
            string title)
        {
            if (File.Exists(path))
            {
                return;
            }

            using (Bitmap bitmap = new Bitmap(600, 900))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (LinearGradientBrush gradient = new LinearGradientBrush(
                new Rectangle(0, 0, 600, 900),
                Color.FromArgb(22, 32, 54),
                Color.FromArgb(72, 48, 112),
                60))
            using (Font headingFont = new Font(
                "Segoe UI",
                28,
                FontStyle.Bold))
            using (Font titleFont = new Font(
                "Segoe UI",
                52,
                FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                graphics.FillRectangle(gradient, 0, 0, 600, 900);
                graphics.DrawString(
                    heading,
                    headingFont,
                    textBrush,
                    new PointF(52, 90));
                graphics.DrawString(
                    title,
                    titleFont,
                    textBrush,
                    new RectangleF(52, 330, 500, 260));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                bitmap.Save(path, ImageFormat.Png);
            }
        }

        private static int TableCount(
            SQLiteConnection connection,
            string table)
        {
            using (SQLiteCommand command = new SQLiteCommand(
                $"select count(*) from [{table}]",
                connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static void ExecuteDemoInsert(
            SQLiteConnection connection,
            string sql,
            IDictionary<string, object> parameters)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.AddWithValue(
                        parameter.Key,
                        parameter.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path)
                    && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }

        private static void TryDeleteDirectory(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path)
                    && Directory.Exists(path)
                    && Path.GetFullPath(path).StartsWith(
                        Path.GetFullPath(dataDirectory)
                            .TrimEnd(Path.DirectorySeparatorChar)
                            + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
            }
        }

        private static void EnsureInitialized()
        {
            if (!string.IsNullOrWhiteSpace(dataDirectory))
            {
                return;
            }

            string fallback = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "Media_Manager");
            Initialize(fallback);
        }

        private sealed class PathDescriptor
        {
            public PathDescriptor(
                string table,
                string column,
                bool isDirectory)
            {
                Table = table;
                Column = column;
                IsDirectory = isDirectory;
            }

            public string Table { get; }
            public string Column { get; }
            public bool IsDirectory { get; }
        }
    }
}
