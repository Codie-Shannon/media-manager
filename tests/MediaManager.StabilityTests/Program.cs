using System;
using System.Data.SQLite;
using System.IO;
using Media_Manager;
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
                Console.WriteLine("PASS: Group 3 stability tests");
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
    }
}
