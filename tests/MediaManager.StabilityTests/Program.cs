using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Media_Manager;
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
                Console.WriteLine("PASS: Group 3 and Group 4 stability tests");
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
