using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using MediaInfo.DotNetWrapper.Enumerations;
using MediaControlsLibrary.Types;
using Media_Manager.Metadata;
using Media_Manager.Models;
using DrawingSize = System.Drawing.Size;

namespace Media_Manager
{
    public static class Fetcher
    {
        private static readonly MediaInfo.DotNetWrapper.MediaInfo MediaInfo =
            new MediaInfo.DotNetWrapper.MediaInfo();
        private static string path;
        private static string baseDirectory;
        private static MediaMetadata metadata;

        public static async Task<bool> ConfigureFetcherAsync(
            MediaType type,
            string filepath,
            string basedirectory = "",
            string defaultlink = "",
            string imdblink = "")
        {
            path = filepath ?? string.Empty;
            baseDirectory = basedirectory ?? string.Empty;
            metadata = null;

            if ((type == MediaType.Movies
                    || type == MediaType.Episodes
                    || type == MediaType.Null)
                && !string.IsNullOrWhiteSpace(path)
                && File.Exists(path))
            {
                MediaInfo.Open(path);
                MediaInfo.Option("ParseSpeed", "0");
            }

            if (type != MediaType.Movies
                && type != MediaType.TVShows
                && type != MediaType.Seasons
                && type != MediaType.Episodes
                && type != MediaType.Games)
            {
                return true;
            }

            string reference = !string.IsNullOrWhiteSpace(imdblink)
                ? imdblink
                : defaultlink;
            if (string.IsNullOrWhiteSpace(reference))
            {
                return true;
            }

            try
            {
                metadata = await MetadataService.GetDetailsAsync(
                    type,
                    reference,
                    CancellationToken.None);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (MetadataProviderException)
            {
                return true;
            }
        }

        public static void Close()
        {
            metadata = null;
        }

        public static Movie GetMovie(Movie movie, int id)
        {
            MediaMetadata details = metadata ?? EmptyMetadata(MediaType.Movies);
            return new Movie
            {
                Id = id,
                OwnerId = movie.OwnerId,
                FilePath = movie.FilePath,
                CustomName = movie.CustomName,
                Name = First(details.Name, movie.CustomName, GetFileName(), "Untitled Movie"),
                Width = GetVideoWidth(),
                Height = GetVideoHeight(),
                Duration = GetDuration(StreamKind.Video),
                Framerate = GetFramerate(),
                Format = GetFormat(),
                FileSize = GetFileSize(),
                CreationTime = GetCreationTime(),
                CreationDate = GetCreationDate(),
                SampleRate = GetSampleRate(),
                AudioChannels = GetAudioChannels(),
                FramerateMode = GetFramerateMode(),
                ReleaseDate = FormatProviderDate(details.ReleaseDate),
                AgeRating = details.AgeRating,
                Genres = Copy(details.Genres),
                Stars = Copy(details.Stars),
                Directors = Copy(details.Directors),
                Writers = Copy(details.Writers),
                ProductionCompanies = Copy(details.ProductionCompanies),
                IMDBLink = movie.IMDBLink,
                Region = details.Region,
                MetaCriticLink = string.Empty,
                UserScore = details.UserScore,
                UserReviewCount = details.UserReviewCount,
                CriticScore = details.CriticScore,
                CriticReviewCount = details.CriticReviewCount,
                CoverImage = DownloadArtwork(MediaType.Movies, id, details.ArtworkUrl)
            };
        }

        public static TVShowFolder GetTVShowFolderAsync(
            TVShowFolder folder,
            int id)
        {
            MediaMetadata details = metadata ?? EmptyMetadata(MediaType.TVShows);
            return new TVShowFolder
            {
                Id = id,
                OwnerId = folder.OwnerId,
                Type = nameof(MediaType.TVShows),
                FolderType = nameof(FolderType.TVShowFolders),
                CustomName = folder.CustomName,
                Name = First(details.Name, folder.CustomName, "Untitled TV Show"),
                CustomCoverImage = GetCustomCoverImage(
                    MediaType.TVShows,
                    id,
                    folder.CustomCoverImage,
                    "custom"),
                CreationTime = GetCreationTime(string.Empty, true),
                CreationDate = GetCreationDate(string.Empty, true),
                MetaCriticLink = string.Empty,
                UserScore = details.UserScore,
                UserReviewCount = details.UserReviewCount,
                CriticScore = details.CriticScore,
                CriticReviewCount = details.CriticReviewCount,
                ReleaseDate = FormatProviderDate(details.ReleaseDate),
                AgeRating = details.AgeRating,
                ProductionCompanies = Copy(details.ProductionCompanies),
                SeasonCount = details.SeasonCount,
                EpisodeCount = details.EpisodeCount,
                Genres = Copy(details.Genres),
                Stars = Copy(details.Stars),
                ReleasePeriod = details.ReleasePeriod,
                IMDBLink = folder.IMDBLink,
                Region = details.Region,
                Creators = Copy(details.Creators),
                CoverImage = DownloadArtwork(
                    MediaType.TVShows,
                    id,
                    details.ArtworkUrl)
            };
        }

        public static SeasonFolder GetSeasonFolder(
            SeasonFolder folder,
            int id,
            string tvshowcustomcoverimage)
        {
            MediaMetadata details = metadata ?? EmptyMetadata(MediaType.Seasons);
            return new SeasonFolder
            {
                Id = id,
                OwnerId = folder.OwnerId,
                Type = nameof(MediaType.TVShows),
                FolderType = nameof(FolderType.SeasonFolders),
                Name = First(details.Name, $"Season {folder.SeasonNumber}"),
                CoverImage = DownloadArtwork(
                    MediaType.Seasons,
                    folder.OwnerId,
                    details.ArtworkUrl,
                    folder.SeasonNumber.ToString(CultureInfo.InvariantCulture)),
                CustomCoverImage = string.IsNullOrEmpty(folder.CustomCoverImage)
                    ? tvshowcustomcoverimage
                    : GetCustomCoverImage(
                        MediaType.Seasons,
                        folder.OwnerId,
                        folder.CustomCoverImage,
                        $"{folder.SeasonNumber} custom"),
                FilePath = folder.FilePath,
                SeasonNumber = folder.SeasonNumber,
                CreationTime = GetCreationTime(string.Empty, true),
                CreationDate = GetCreationDate(string.Empty, true),
                ReleaseDate = FormatProviderDate(details.ReleaseDate),
                EpisodeCount = details.EpisodeCount,
                IMDBLink = folder.IMDBLink,
                MetaCriticLink = string.Empty
            };
        }

        public static Episode GetEpisode(
            TVShowFolder folder,
            Episode episode,
            int id,
            int episodenumber,
            string imdblink,
            string metacritic)
        {
            MediaMetadata details = metadata ?? EmptyMetadata(MediaType.Episodes);
            return new Episode
            {
                Id = id,
                OwnerId = episode.OwnerId,
                CustomName = episode.CustomName,
                Name = First(
                    details.Name,
                    episode.CustomName,
                    $"Episode {episodenumber}"),
                CoverImage = First(
                    DownloadArtwork(
                        MediaType.Episodes,
                        id,
                        details.ArtworkUrl),
                    episode.CoverImage),
                FilePath = episode.FilePath,
                Season = episode.Season,
                EpisodeNumber = episodenumber,
                Width = GetVideoWidth(),
                Height = GetVideoHeight(),
                Duration = GetDuration(StreamKind.Video),
                Framerate = GetFramerate(),
                Format = GetFormat(),
                FileSize = GetFileSize(),
                CreationTime = GetCreationTime(),
                CreationDate = GetCreationDate(),
                SampleRate = GetSampleRate(),
                AudioChannels = GetAudioChannels(),
                FramerateMode = GetFramerateMode(),
                AirDate = FormatProviderDate(details.ReleaseDate),
                Region = First(details.Region, folder?.Region),
                AgeRating = First(details.AgeRating, folder?.AgeRating),
                Directors = Copy(details.Directors),
                Writers = Copy(details.Writers),
                ProductionCompanies = CopyOr(details.ProductionCompanies, folder?.ProductionCompanies),
                IMDBLink = imdblink,
                Genres = CopyOr(details.Genres, folder?.Genres),
                Stars = CopyOr(details.Stars, folder?.Stars),
                MetaCriticLink = string.Empty
            };
        }

        public static Video GetVideo(int id, Video video)
        {
            string width = GetVideoWidth();
            string height = GetVideoHeight();
            return new Video
            {
                Id = id,
                OwnerId = video.OwnerId,
                FilePath = path,
                CoverImage = GetVideoPreview(path, id, width, height),
                CustomName = video.CustomName,
                Name = GetFileName(),
                Width = width,
                Height = height,
                Duration = GetDuration(StreamKind.Video),
                Framerate = GetFramerate(),
                Format = GetFormat(),
                FileSize = GetFileSize(),
                CreationTime = GetCreationTime(),
                CreationDate = GetCreationDate(),
                SampleRate = GetSampleRate(),
                AudioChannels = GetAudioChannels(),
                FramerateMode = GetFramerateMode()
            };
        }

        public static Picture GetPicture(int id, Picture picture)
        {
            string width = GetPictureWidth();
            string height = GetPictureHeight();
            int widthValue;
            int heightValue;
            int.TryParse(width, out widthValue);
            int.TryParse(height, out heightValue);
            DrawingSize size = new DrawingSize(Math.Max(1, widthValue), Math.Max(1, heightValue));
            return new Picture
            {
                Id = id,
                OwnerId = picture.OwnerId,
                FilePath = path,
                CoverImage = GetPicturePreview(path, id, size),
                CustomName = picture.CustomName,
                Name = GetFileName(),
                Width = width,
                Height = height,
                Format = GetFormat(),
                FileSize = GetFileSize(),
                CreationTime = GetCreationTime(),
                CreationDate = GetCreationDate(),
                ColourSpace = GetColourSpace(),
                BitDepth = GetBitDepth(),
                CompMode = GetCompMode(StreamKind.Image)
            };
        }

        public static Song GetSong(int id, Song song)
        {
            return new Song
            {
                Id = id,
                OwnerId = song.OwnerId,
                FilePath = path,
                CoverImage = string.Empty,
                Name = GetFileName(),
                Duration = GetDuration(StreamKind.Audio),
                Format = GetFormat(),
                FileSize = GetFileSize(),
                CreationTime = GetCreationTime(),
                CreationDate = GetCreationDate(),
                SampleRate = GetSampleRate(),
                AudioChannels = GetAudioChannels(),
                CompMode = GetCompMode(StreamKind.Audio)
            };
        }

        public static Game GetGame(int id, Game game)
        {
            MediaMetadata details = metadata ?? EmptyMetadata(MediaType.Games);
            return new Game
            {
                Id = id,
                OwnerId = game.OwnerId,
                FilePath = game.FilePath,
                BaseDirectory = game.BaseDirectory,
                CoverImage = DownloadArtwork(
                    MediaType.Games,
                    id,
                    details.ArtworkUrl),
                CustomName = game.CustomName,
                Name = First(details.Name, game.CustomName, GetFileName(), "Untitled Game"),
                Format = GetFormat(),
                FileSize = GetFileSize(baseDirectory),
                CreationTime = GetCreationTime(baseDirectory),
                CreationDate = GetCreationDate(baseDirectory),
                IGDBLink = game.IGDBLink,
                Publisher = details.Publisher,
                ReleaseDate = FormatProviderDate(details.ReleaseDate),
                Type = First(details.Type, game.Type, "Game"),
                UserScore = details.UserScore,
                UserReviewCount = details.UserReviewCount,
                CriticScore = details.CriticScore,
                CriticReviewCount = details.CriticReviewCount,
                Genres = Copy(details.Genres),
                AvailablePlatforms = Copy(details.Platforms)
            };
        }

        public static Song UpdateSong(Song selectedsong, Song song)
        {
            selectedsong.CustomName = song.CustomName;
            selectedsong.CoverImage = GetMusicCover(
                song.CoverImage,
                selectedsong.CoverImage,
                selectedsong.Id);
            selectedsong.OwnerId = song.OwnerId;
            return selectedsong;
        }

        public static string GetCustomCoverImage(
            MediaType type,
            int id,
            string cover,
            string extension = "")
        {
            return string.IsNullOrEmpty(cover)
                ? string.Empty
                : SaveImage(type, id, cover, extension);
        }

        public static string GetIMDBSeasonLink(string reference, int seasonnumber)
        {
            return MetadataService.SeasonReference(reference, seasonnumber);
        }

        public static string GetMetacriticSeasonLink(string reference, int seasonnumber)
        {
            return string.Empty;
        }

        public static Task<string[]> GetEpisodeLinksAsync(
            string metacriticlink,
            string seasonReference,
            int episodenumber)
        {
            string episodeReference = MetadataService.EpisodeReference(
                seasonReference,
                episodenumber);
            return Task.FromResult(new[] { string.Empty, episodeReference });
        }

        public static int GetEpisodeNumber(string filepath)
        {
            Regex separator = new Regex("[_:.()]|[-]|[[]|[]]");
            string value = separator.Replace(
                Path.GetFileNameWithoutExtension(filepath).ToLowerInvariant(),
                " ");
            Match match = new Regex(
                @"(?:e|x|ep|episode)\s?(?<episode>\d+)",
                RegexOptions.IgnoreCase).Match(value);
            int episode;
            return match.Success
                && int.TryParse(match.Groups["episode"].Value, out episode)
                    ? episode
                    : 0;
        }

        public static List<FileInfo> GetEpisodes(
            DirectoryInfo dirinfo,
            string[] extensions)
        {
            if (dirinfo == null || !dirinfo.Exists)
            {
                return new List<FileInfo>();
            }

            HashSet<string> allowed = new HashSet<string>(
                extensions ?? new string[0],
                StringComparer.OrdinalIgnoreCase);
            return dirinfo.EnumerateFiles()
                .Where(file => allowed.Contains(file.Extension))
                .ToList();
        }

        private static MediaMetadata EmptyMetadata(MediaType kind)
        {
            return new MediaMetadata { Kind = kind };
        }

        private static string DownloadArtwork(
            MediaType type,
            int id,
            string artworkUrl,
            string extension = "")
        {
            if (string.IsNullOrWhiteSpace(artworkUrl))
            {
                return string.Empty;
            }

            try
            {
                return SaveImage(type, id, artworkUrl, extension);
            }
            catch (Exception exception) when (
                exception is WebException
                || exception is IOException
                || exception is ArgumentException
                || exception is ExternalException)
            {
                return string.Empty;
            }
        }

        private static string SaveImage(
            MediaType type,
            int id,
            string source,
            string extension)
        {
            string directory = $"{Properties.Settings.Default[type.ToString()]}";
            Directory.CreateDirectory(directory);
            string suffix = string.IsNullOrWhiteSpace(extension)
                ? string.Empty
                : " " + extension.Trim();
            string temporary = Path.Combine(directory, $"{id}_temp.png");
            string destination = Path.Combine(directory, $"{id}{suffix}.png");

            if (Uri.TryCreate(source, UriKind.Absolute, out Uri uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "MediaManager/1.0";
                    client.DownloadFile(uri, temporary);
                }
            }
            else
            {
                File.Copy(source, temporary, true);
            }

            ResizeAndSave(
                temporary,
                destination,
                new DrawingSize(199, 291),
                false,
                DrawingSize.Empty);
            TryDelete(temporary);
            return destination;
        }

        private static void ResizeAndSave(
            string temporary,
            string destination,
            DrawingSize size,
            bool preserveAspect,
            DrawingSize desiredSize)
        {
            using (Bitmap original = new Bitmap(temporary))
            using (Bitmap resized = ResizeImage(
                original,
                size,
                preserveAspect,
                desiredSize))
            {
                resized.Save(destination);
            }
        }

        private static Bitmap ResizeImage(
            Bitmap image,
            DrawingSize size,
            bool preserveAspect,
            DrawingSize desiredSize)
        {
            if (preserveAspect && desiredSize.Width > 0 && desiredSize.Height > 0)
            {
                decimal widthRatio = (decimal)size.Width / desiredSize.Width;
                decimal heightRatio = (decimal)size.Height / desiredSize.Height;
                decimal ratio = Math.Max(widthRatio, heightRatio);
                if (ratio > 0)
                {
                    size = new DrawingSize(
                        Math.Max(1, (int)(size.Width / ratio)),
                        Math.Max(1, (int)(size.Height / ratio)));
                }
            }

            Bitmap bitmap = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height));
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
            }

            return bitmap;
        }

        private static string GetFileName()
        {
            return string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : Path.GetFileNameWithoutExtension(path);
        }

        private static string GetPictureWidth()
        {
            return MediaInfo.Get(StreamKind.Image, 0, "Width");
        }

        private static string GetPictureHeight()
        {
            return MediaInfo.Get(StreamKind.Image, 0, "Height");
        }

        private static string GetVideoWidth()
        {
            return MediaInfo.Get(StreamKind.Video, 0, "Width");
        }

        private static string GetVideoHeight()
        {
            return MediaInfo.Get(StreamKind.Video, 0, "Height");
        }

        private static string GetFormat()
        {
            return string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : Path.GetExtension(path).TrimStart('.').ToUpperInvariant();
        }

        private static double GetFileSize(string directory = "")
        {
            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                try
                {
                    return new DirectoryInfo(directory)
                        .EnumerateFiles("*", SearchOption.AllDirectories)
                        .Sum(file => (double)file.Length);
                }
                catch (IOException)
                {
                    return 0;
                }
                catch (UnauthorizedAccessException)
                {
                    return 0;
                }
            }

            double result;
            return double.TryParse(
                MediaInfo.Get(StreamKind.General, 0, "FileSize"),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result)
                    ? result
                    : 0;
        }

        private static string GetCreationTime(
            string directory = "",
            bool ispresent = false)
        {
            if (ispresent)
            {
                return DateTime.Now.ToString("HH-mm-ss");
            }

            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                return Directory.GetCreationTime(directory).ToString("HH-mm-ss");
            }

            return !string.IsNullOrWhiteSpace(path) && File.Exists(path)
                ? File.GetCreationTime(path).ToString("HH-mm-ss")
                : string.Empty;
        }

        private static string GetCreationDate(
            string directory = "",
            bool ispresent = false)
        {
            if (ispresent)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                return Directory.GetCreationTime(directory).ToString("yyyy-MM-dd");
            }

            return !string.IsNullOrWhiteSpace(path) && File.Exists(path)
                ? File.GetCreationTime(path).ToString("yyyy-MM-dd")
                : string.Empty;
        }

        private static string GetCompMode(StreamKind kind)
        {
            return MediaInfo.Get(kind, 0, "Compression_Mode");
        }

        private static string GetPicturePreview(
            string filepath,
            int id,
            DrawingSize size)
        {
            string directory = Properties.Settings.Default.Pictures;
            Directory.CreateDirectory(directory);
            string temporary = Path.Combine(directory, $"{id}_temp.png");
            string destination = Path.Combine(directory, $"{id}.png");
            File.Copy(filepath, temporary, true);
            ResizeAndSave(
                temporary,
                destination,
                size,
                true,
                new DrawingSize(401, 230));
            TryDelete(temporary);
            return destination;
        }

        private static string GetColourSpace()
        {
            return MediaInfo.Get(StreamKind.Image, 0, "ColorSpace");
        }

        private static string GetBitDepth()
        {
            return MediaInfo.Get(StreamKind.Image, 0, "BitDepth");
        }

        private static double GetDuration(StreamKind kind)
        {
            double result;
            return double.TryParse(
                MediaInfo.Get(kind, 0, "Duration"),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result)
                    ? result
                    : 0;
        }

        private static double GetSampleRate()
        {
            string value = MediaInfo.Get(StreamKind.Audio, 0, "SamplingRate");
            value = value?.Split(' ').FirstOrDefault();
            double result;
            return double.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result)
                    ? result
                    : 0;
        }

        private static string GetAudioChannels()
        {
            string channels = MediaInfo.Get(StreamKind.Audio, 0, "Channels");
            return channels == "0" ? string.Empty : channels;
        }

        private static string GetVideoPreview(
            string filepath,
            int id,
            string widthText,
            string heightText)
        {
            int width;
            int height;
            if (!int.TryParse(widthText, out width)
                || !int.TryParse(heightText, out height)
                || width <= 0
                || height <= 0)
            {
                return string.Empty;
            }

            string directory = Properties.Settings.Default.Videos;
            Directory.CreateDirectory(directory);
            string temporary = Path.Combine(directory, $"{id}_temp.png");
            string destination = Path.Combine(directory, $"{id}.png");
            MediaPlayer player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };
            try
            {
                player.Open(new Uri(filepath));
                player.Pause();
                player.Position = TimeSpan.Zero;
                Thread.Sleep(1000);
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext context = visual.RenderOpen())
                {
                    context.DrawVideo(player, new Rect(0, 0, width, height));
                }

                RenderTargetBitmap bitmap = new RenderTargetBitmap(
                    width,
                    height,
                    96,
                    96,
                    PixelFormats.Pbgra32);
                bitmap.Render(visual);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (Stream stream = File.Create(temporary))
                {
                    encoder.Save(stream);
                }
            }
            finally
            {
                player.Close();
            }

            ResizeAndSave(
                temporary,
                destination,
                new DrawingSize(width, height),
                true,
                new DrawingSize(401, 230));
            TryDelete(temporary);
            return destination;
        }

        private static double GetFramerate()
        {
            double result;
            return double.TryParse(
                MediaInfo.Get(StreamKind.Video, 0, "FrameRate"),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result)
                    ? result
                    : 0;
        }

        private static string GetFramerateMode()
        {
            return MediaInfo.Get(StreamKind.Video, 0, "FrameRate_Mode");
        }

        private static string GetMusicCover(
            string filepath,
            string oldfilepath,
            int id)
        {
            if (filepath == oldfilepath)
            {
                return oldfilepath;
            }

            if (!Validation.File(filepath))
            {
                TryDelete(oldfilepath);
                return string.Empty;
            }

            MediaInfo.Open(filepath);
            MediaInfo.Option("ParseSpeed", "0");
            int width;
            int height;
            int.TryParse(GetPictureWidth(), out width);
            int.TryParse(GetPictureHeight(), out height);
            string directory = Properties.Settings.Default.Music;
            Directory.CreateDirectory(directory);
            string temporary = Path.Combine(directory, $"{id}_temp.png");
            string destination = Path.Combine(directory, $"{id}.png");
            File.Copy(filepath, temporary, true);
            ResizeAndSave(
                temporary,
                destination,
                new DrawingSize(Math.Max(1, width), Math.Max(1, height)),
                true,
                new DrawingSize(401, 230));
            TryDelete(temporary);
            return destination;
        }

        private static string FormatProviderDate(string value)
        {
            DateTime date;
            return DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date)
                    ? date.ToString("MMMM d, yyyy")
                    : value ?? string.Empty;
        }

        private static string First(params string[] values)
        {
            return values?.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                ?? string.Empty;
        }

        private static List<string> Copy(IEnumerable<string> values)
        {
            return values?.Where(value => !string.IsNullOrWhiteSpace(value)).ToList()
                ?? new List<string>();
        }

        private static List<string> CopyOr(
            IEnumerable<string> preferred,
            IEnumerable<string> fallback)
        {
            List<string> values = Copy(preferred);
            return values.Count > 0 ? values : Copy(fallback);
        }

        private static void TryDelete(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return;
            }

            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
