using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Media_Manager;
using Media_Manager.Data;
using Media_Manager.Metadata;
using Media_Manager.Models;

namespace MediaManager.StabilityTests
{
    internal static class Program
    {
        private static int Main()
        {
            string testDirectory = Path.Combine(
                Path.GetTempPath(),
                "MediaManagerStabilityTests",
                Guid.NewGuid().ToString("N"));

            try
            {
                TestTVShowOwnershipIsolation(testDirectory);
                TestNullAndMissingMetadataFormatting();
                TestProviderArchitectureAsync(testDirectory).GetAwaiter().GetResult();
                TestDataReliability(testDirectory);
                Console.WriteLine(
                    "PASS: Group 3, Group 4, and Group 5 stability tests");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"FAIL: {exception}");
                return 1;
            }
            finally
            {
                if (Directory.Exists(testDirectory))
                {
                    Directory.Delete(testDirectory, true);
                }
            }
        }

        private static void TestTVShowOwnershipIsolation(string testDirectory)
        {
            Database.Initialize(testDirectory);
            string databasePath = Path.Combine(testDirectory, "MediaManagerDB.db");
            using (SQLiteConnection connection = Open(databasePath))
            {
                Execute(connection, @"
insert into TVShowFolders (Id, OwnerId, isFavourite, Name, Type, FolderType)
values (101, 0, 0, 'Synthetic Show A', 'TVShows', 'TVShowFolders');
insert into TVShowFolders (Id, OwnerId, isFavourite, Name, Type, FolderType)
values (102, 0, 0, 'Synthetic Show B', 'TVShows', 'TVShowFolders');
insert into SeasonFolders
    (Id, OwnerId, FilePath, Name, Type, FolderType, SeasonNumber)
values
    (201, 101, 'synthetic-show-a-season', 'Season 1', 'TVShows', 'SeasonFolders', 1);
insert into SeasonFolders
    (Id, OwnerId, FilePath, Name, Type, FolderType, SeasonNumber)
values
    (202, 102, 'synthetic-show-b-season', 'Season 1', 'TVShows', 'SeasonFolders', 1);
insert into Episodes
    (Id, OwnerId, FilePath, CoverImage, Name, Season, EpisodeNumber,
     Width, Height, Duration, Framerate, Format, FileSize, CreationTime, CreationDate)
values
    (301, 201, 'synthetic-a.mp4', '', 'Synthetic Episode A', 1, 1,
     '1920', '1080', 1000, 24, 'MP4', 1, '12-00-00', '2026-07-26');
insert into Episodes
    (Id, OwnerId, FilePath, CoverImage, Name, Season, EpisodeNumber,
     Width, Height, Duration, Framerate, Format, FileSize, CreationTime, CreationDate)
values
    (302, 202, 'synthetic-b.mp4', '', 'Synthetic Episode B', 1, 1,
     '1920', '1080', 1000, 24, 'MP4', 1, '12-00-00', '2026-07-26');");
            }

            Database.RemoveTVShowFolder(new TVShowFolder { Id = 101, OwnerId = 0 });
            Database.RemoveTVShowFolder(null);
            using (SQLiteConnection connection = Open(databasePath))
            {
                AssertCount(connection, "TVShowFolders", 101, 0);
                AssertCount(connection, "SeasonFolders", 201, 0);
                AssertCount(connection, "Episodes", 301, 0);
                AssertCount(connection, "TVShowFolders", 102, 1);
                AssertCount(connection, "SeasonFolders", 202, 1);
                AssertCount(connection, "Episodes", 302, 1);
            }
        }

        private static void TestNullAndMissingMetadataFormatting()
        {
            Formatter.FormatImage(MediaType.Videos, null);
            AssertEqual(string.Empty, Formatter.FormatDate(null, "yyyy-MM-dd", "D"));
            AssertEqual("not-a-date", Formatter.FormatDate("not-a-date", "yyyy-MM-dd", "D"));
            AssertEqual(string.Empty, Formatter.FormatGameReleaseDate(null));
            AssertEqual(string.Empty, Formatter.FormatGameReleaseDate("not-a-date"));
            AssertEqual(string.Empty, Formatter.FormatVirtualEntertainmentReleaseDate(null, null));
        }

        private static async Task TestProviderArchitectureAsync(string testDirectory)
        {
            await TestTmdbProviderAsync();
            await TestLegacyImdbReferenceAsync();
            await TestIgdbProviderAsync();
            await TestCancellationAsync();
            TestMetadataCache(testDirectory);
            await TestEncryptedSettingsAndManualFallbackAsync(testDirectory);
        }

        private static void TestDataReliability(string testDirectory)
        {
            string dataDirectory = Path.Combine(testDirectory, "data-reliability");
            ApplicationLog.Initialize(dataDirectory);
            LibraryDataService.Initialize(dataDirectory);
            Database.Initialize(dataDirectory);

            string mediaDirectory = Path.Combine(dataDirectory, "SyntheticMedia");
            string imageDirectory = Path.Combine(dataDirectory, "Images", "Video Preview");
            Directory.CreateDirectory(mediaDirectory);
            Directory.CreateDirectory(imageDirectory);
            string existingFile = Path.Combine(mediaDirectory, "existing.mp4");
            string missingFile = Path.Combine(mediaDirectory, "missing.mp4");
            string cover = Path.Combine(imageDirectory, "cover.png");
            File.WriteAllText(existingFile, "synthetic media");
            File.WriteAllText(cover, "synthetic cover");

            using (SQLiteConnection connection = Open(Database.DatabasePath))
            {
                Execute(connection, $@"
insert into Videos
    (Id, OwnerId, isFavourite, FilePath, CoverImage, Name, Width, Height,
     Duration, Framerate, Format, FileSize, CreationTime, CreationDate)
values
    (1, 0, 0, '{Sql(existingFile)}', '{Sql(cover)}', 'Existing One',
     '1920', '1080', 1, 24, 'MP4', 1, '12:00:00', '2026-01-01');
insert into Videos
    (Id, OwnerId, isFavourite, FilePath, CoverImage, Name, Width, Height,
     Duration, Framerate, Format, FileSize, CreationTime, CreationDate)
values
    (2, 0, 0, '{Sql(existingFile)}', '{Sql(cover)}', 'Existing Duplicate',
     '1920', '1080', 1, 24, 'MP4', 1, '12:00:00', '2026-01-01');
insert into Videos
    (Id, OwnerId, isFavourite, FilePath, CoverImage, Name, Width, Height,
     Duration, Framerate, Format, FileSize, CreationTime, CreationDate)
values
    (3, 0, 0, '{Sql(missingFile)}', '{Sql(cover)}', 'Missing File',
     '1920', '1080', 1, 24, 'MP4', 1, '12:00:00', '2026-01-01');");
                using (SQLiteCommand version = new SQLiteCommand(
                    "PRAGMA user_version;",
                    connection))
                {
                    AssertEqual(
                        Database.SchemaVersion.ToString(),
                        $"{version.ExecuteScalar()}");
                }
            }

            LibraryHealthReport health = LibraryDataService.CheckLibrary(
                CancellationToken.None);
            AssertEqual("1", health.MissingPaths.ToString());
            AssertEqual("1", health.DuplicatePaths.ToString());

            string catalog = Path.Combine(testDirectory, "catalog.json");
            LibraryDataService.ExportCatalog(catalog);
            string catalogText = File.ReadAllText(catalog);
            if (catalogText.IndexOf(
                    dataDirectory,
                    StringComparison.OrdinalIgnoreCase) >= 0
                || catalogText.IndexOf(
                    "sample://Videos/",
                    StringComparison.Ordinal) < 0)
            {
                throw new InvalidOperationException(
                    "Privacy-safe catalog paths were not redacted.");
            }

            string backup = Path.Combine(testDirectory, "library.mmbak");
            LibraryDataService.CreateBackup(backup);
            if (!File.Exists(backup))
            {
                throw new InvalidOperationException(
                    "Library backup was not created.");
            }

            using (SQLiteConnection connection = Open(Database.DatabasePath))
            {
                Execute(connection, "delete from Videos;");
            }
            File.Delete(cover);
            LibraryDataService.RestoreBackup(backup);
            using (SQLiteConnection connection = Open(Database.DatabasePath))
            {
                AssertCount(connection, "Videos", 1, 1);
                AssertCount(connection, "Videos", 2, 1);
                AssertCount(connection, "Videos", 3, 1);
            }
            if (!File.Exists(cover))
            {
                throw new InvalidOperationException(
                    "Managed cover image was not restored.");
            }

            string automatic =
                LibraryDataService.CreateAutomaticBackupIfDue();
            string sameAutomatic =
                LibraryDataService.CreateAutomaticBackupIfDue();
            AssertEqual(automatic, sameAutomatic);

            File.WriteAllText(Database.DatabasePath, "corrupt database");
            if (!LibraryDataService.RecoverDatabaseIfRequired())
            {
                throw new InvalidOperationException(
                    "Corrupt database was not recovered.");
            }
            Database.Initialize(dataDirectory);
            using (SQLiteConnection connection = Open(Database.DatabasePath))
            {
                AssertCount(connection, "Videos", 1, 1);
            }

            using (SQLiteConnection connection = Open(Database.DatabasePath))
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                for (int index = 10; index < 2510; index++)
                {
                    using (SQLiteCommand command = new SQLiteCommand(
                        @"insert into Videos
                        (Id, OwnerId, isFavourite, FilePath, CoverImage, Name,
                         Width, Height, Duration, Framerate, Format, FileSize,
                         CreationTime, CreationDate)
                        values
                        (@id, 0, 0, @path, '', @name, '1', '1', 1, 1,
                         'MP4', 1, '12:00:00', '2026-01-01')",
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue("@id", index);
                        command.Parameters.AddWithValue(
                            "@path",
                            Path.Combine(
                                mediaDirectory,
                                $"large-{index}.mp4"));
                        command.Parameters.AddWithValue(
                            "@name",
                            $"Synthetic {index}");
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            Stopwatch scanTimer = Stopwatch.StartNew();
            LibraryHealthReport largeHealth =
                LibraryDataService.CheckLibrary(CancellationToken.None);
            scanTimer.Stop();
            if (largeHealth.TotalRecords < 2503
                || scanTimer.Elapsed > TimeSpan.FromSeconds(30))
            {
                throw new InvalidOperationException(
                    "Large-library health scan did not complete practically.");
            }

            string invalidBackup = Path.Combine(
                testDirectory,
                "invalid.mmbak");
            File.WriteAllText(invalidBackup, "not a zip");
            try
            {
                LibraryDataService.RestoreBackup(invalidBackup);
                throw new InvalidOperationException(
                    "Invalid backup was accepted.");
            }
            catch (LibraryDataException)
            {
            }

            string traversalBackup = Path.Combine(
                testDirectory,
                "traversal.mmbak");
            string escapedPath = Path.Combine(
                testDirectory,
                "escaped.txt");
            using (ZipArchive archive = ZipFile.Open(
                traversalBackup,
                ZipArchiveMode.Create))
            {
                ZipArchiveEntry entry = archive.CreateEntry(
                    "../escaped.txt");
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    writer.Write("unsafe");
                }
            }
            try
            {
                LibraryDataService.RestoreBackup(traversalBackup);
                throw new InvalidOperationException(
                    "Traversal backup was accepted.");
            }
            catch (LibraryDataException)
            {
            }
            if (File.Exists(escapedPath))
            {
                throw new InvalidOperationException(
                    "Traversal backup escaped its staging directory.");
            }

            string demoDirectory = Path.Combine(testDirectory, "demo");
            ApplicationLog.Initialize(demoDirectory);
            LibraryDataService.Initialize(demoDirectory);
            Database.Initialize(demoDirectory);
            LibraryDataService.EnsureDemoLibrary();
            LibraryHealthReport demoHealth =
                LibraryDataService.CheckLibrary(CancellationToken.None);
            if (demoHealth.TotalRecords < 5 || demoHealth.MissingPaths != 0)
            {
                throw new InvalidOperationException(
                    "Synthetic demo profile is incomplete.");
            }

            string futureSchemaDirectory = Path.Combine(
                testDirectory,
                "future-schema");
            Database.Initialize(futureSchemaDirectory);
            using (SQLiteConnection connection = Open(Database.DatabasePath))
            {
                Execute(
                    connection,
                    $"PRAGMA user_version = {Database.SchemaVersion + 1};");
            }
            try
            {
                Database.Initialize(futureSchemaDirectory);
                throw new InvalidOperationException(
                    "A database from a newer application version was accepted.");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.IndexOf(
                        "newer version",
                        StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw;
                }
            }
        }

        private static async Task TestTmdbProviderAsync()
        {
            QueueTransport transport = new QueueTransport(
                "{\"results\":[{\"id\":603,\"title\":\"The Matrix\",\"poster_path\":\"/matrix.jpg\",\"release_date\":\"1999-03-30\"}]}",
                "{\"id\":603,\"title\":\"The Matrix\",\"poster_path\":\"/matrix.jpg\",\"release_date\":\"1999-03-30\",\"vote_average\":8.2,\"vote_count\":26000,\"genres\":[{\"name\":\"Action\"}],\"production_companies\":[{\"name\":\"Village Roadshow Pictures\"}],\"credits\":{\"cast\":[{\"name\":\"Keanu Reeves\"}],\"crew\":[{\"name\":\"Lana Wachowski\",\"job\":\"Director\"},{\"name\":\"Lilly Wachowski\",\"job\":\"Writer\"}]},\"release_dates\":{\"results\":[{\"iso_3166_1\":\"NZ\",\"release_dates\":[{\"certification\":\"M\"}]}]}}");
            IMetadataProvider provider = new TmdbMetadataProvider(
                "synthetic-token",
                transport);
            IReadOnlyList<MetadataSearchResult> results = await provider.SearchAsync(
                new MetadataSearchRequest
                {
                    Kind = MediaType.Movies,
                    Query = "The Matrix"
                },
                CancellationToken.None);
            AssertEqual("The Matrix", results[0].Name);
            AssertEqual("https://www.themoviedb.org/movie/603", results[0].ExternalUrl);

            MediaMetadata details = await provider.GetDetailsAsync(
                "603",
                MediaType.Movies,
                CancellationToken.None);
            AssertEqual("The Matrix", details.Name);
            AssertEqual("M", details.AgeRating);
            AssertEqual("Lana Wachowski", details.Directors[0]);
            AssertEqual("Action", details.Genres[0]);
        }

        private static async Task TestIgdbProviderAsync()
        {
            const string game =
                "[{\"id\":1942,\"name\":\"Thief\",\"slug\":\"thief\",\"category\":0,\"cover\":{\"url\":\"//images.igdb.com/igdb/image/upload/t_thumb/co1.jpg\"},\"first_release_date\":762480000,\"rating\":82.5,\"rating_count\":100,\"aggregated_rating\":90.0,\"aggregated_rating_count\":20,\"genres\":[{\"name\":\"Stealth\"}],\"platforms\":[{\"name\":\"PC\"}],\"involved_companies\":[{\"publisher\":true,\"company\":{\"name\":\"Eidos Interactive\"}}]}]";
            QueueTransport transport = new QueueTransport(
                "{\"access_token\":\"synthetic-access\",\"expires_in\":3600,\"token_type\":\"bearer\"}",
                game,
                game);
            IMetadataProvider provider = new IgdbMetadataProvider(
                "synthetic-client",
                "synthetic-secret",
                transport);
            IReadOnlyList<MetadataSearchResult> results = await provider.SearchAsync(
                new MetadataSearchRequest
                {
                    Kind = MediaType.Games,
                    Query = "Thief"
                },
                CancellationToken.None);
            AssertEqual("Thief", results[0].Name);
            AssertEqual("PC", results[0].Platforms[0]);

            MediaMetadata details = await provider.GetDetailsAsync(
                "thief",
                MediaType.Games,
                CancellationToken.None);
            AssertEqual("Eidos Interactive", details.Publisher);
            AssertEqual("Stealth", details.Genres[0]);
            AssertEqual("Main Game", details.Type);
        }

        private static async Task TestLegacyImdbReferenceAsync()
        {
            QueueTransport transport = new QueueTransport(
                "{\"movie_results\":[{\"id\":603}]}",
                "{\"id\":603,\"title\":\"The Matrix\",\"release_date\":\"1999-03-30\",\"credits\":{\"cast\":[],\"crew\":[]},\"release_dates\":{\"results\":[]}}");
            IMetadataProvider provider = new TmdbMetadataProvider(
                "synthetic-token",
                transport);
            MediaMetadata details = await provider.GetDetailsAsync(
                "imdb:tt0133093",
                MediaType.Movies,
                CancellationToken.None);
            AssertEqual("The Matrix", details.Name);
            AssertEqual("603", details.ProviderId);
        }

        private static async Task TestCancellationAsync()
        {
            CancellationTokenSource cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            IMetadataProvider provider = new TmdbMetadataProvider(
                "synthetic-token",
                new CancellationTransport());
            try
            {
                await provider.SearchAsync(
                    new MetadataSearchRequest
                    {
                        Kind = MediaType.Movies,
                        Query = "Cancelled"
                    },
                    cancellation.Token);
                throw new InvalidOperationException("Expected search cancellation.");
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static void TestMetadataCache(string testDirectory)
        {
            MetadataCache cache = new MetadataCache(testDirectory);
            const string key = "synthetic-cache-key";
            cache.Write(key, "Synthetic", new List<string> { "cached" });
            List<string> cached;
            if (!cache.TryRead(key, TimeSpan.FromDays(1), false, out cached))
            {
                throw new InvalidOperationException("Metadata cache miss.");
            }

            AssertEqual("cached", cached[0]);
            string cacheText = File.ReadAllText(
                Directory.GetFiles(
                    Path.Combine(testDirectory, "MetadataCache"),
                    "*.json")[0]);
            if (!cacheText.Contains("\"Provider\": \"Synthetic\"")
                || !cacheText.Contains("\"RetrievedAtUtc\""))
            {
                throw new InvalidOperationException(
                    "Metadata cache provenance is incomplete.");
            }
        }

        private static async Task TestEncryptedSettingsAndManualFallbackAsync(
            string testDirectory)
        {
            string providerDirectory = Path.Combine(testDirectory, "providers");
            MetadataService.Initialize(providerDirectory);
            MetadataService.SaveSettings(
                "synthetic-tmdb-secret",
                "synthetic-igdb-client",
                "synthetic-igdb-secret");
            string settingsText = File.ReadAllText(
                Path.Combine(providerDirectory, "metadata-providers.json"));
            if (settingsText.Contains("synthetic-tmdb-secret")
                || settingsText.Contains("synthetic-igdb-secret"))
            {
                throw new InvalidOperationException(
                    "Provider credentials were stored as plaintext.");
            }

            string manualDirectory = Path.Combine(testDirectory, "manual");
            MetadataService.Initialize(manualDirectory);
            IReadOnlyList<MetadataSearchResult> results =
                await MetadataService.SearchAsync(
                    new MetadataSearchRequest
                    {
                        Kind = MediaType.Movies,
                        Query = "Manual Movie"
                    },
                    CancellationToken.None);
            AssertEqual("Manual", results[0].ProviderName);
            MediaMetadata manual = await MetadataService.GetDetailsAsync(
                MediaType.Movies,
                results[0].ExternalUrl,
                CancellationToken.None);
            AssertEqual("Manual Movie", manual.Name);
        }

        private static SQLiteConnection Open(string databasePath)
        {
            SQLiteConnection connection = new SQLiteConnection(
                $"Data Source={databasePath};Version=3;");
            connection.Open();
            return connection;
        }

        private static void Execute(SQLiteConnection connection, string sql)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void AssertCount(
            SQLiteConnection connection,
            string table,
            int id,
            int expected)
        {
            using (SQLiteCommand command = new SQLiteCommand(
                $"select count(*) from {table} where Id = @id",
                connection))
            {
                command.Parameters.AddWithValue("@id", id);
                int actual = Convert.ToInt32(command.ExecuteScalar());
                if (actual != expected)
                {
                    throw new InvalidOperationException(
                        $"{table} Id {id}: expected {expected}, actual {actual}.");
                }
            }
        }

        private static void AssertEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Expected '{expected}', actual '{actual}'.");
            }
        }

        private static string Sql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private sealed class QueueTransport : IMetadataTransport
        {
            private readonly Queue<string> responses;

            public QueueTransport(params string[] responses)
            {
                this.responses = new Queue<string>(responses);
            }

            public Task<string> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (responses.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No synthetic HTTP response remains.");
                }

                return Task.FromResult(responses.Dequeue());
            }
        }

        private sealed class CancellationTransport : IMetadataTransport
        {
            public Task<string> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult("{}");
            }
        }
    }
}
