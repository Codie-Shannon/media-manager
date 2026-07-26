using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Media_Manager.Metadata
{
    public static class MetadataService
    {
        private static readonly object Sync = new object();
        private static readonly TimeSpan SearchLifetime = TimeSpan.FromHours(24);
        private static readonly TimeSpan DetailLifetime = TimeSpan.FromDays(7);
        private static ProviderSettingsStore settingsStore;
        private static MetadataCache cache;
        private static IMetadataTransport transport;
        private static List<IMetadataProvider> providers;

        public static void Initialize(string localDataDirectory)
        {
            if (string.IsNullOrWhiteSpace(localDataDirectory))
            {
                throw new ArgumentException(
                    "A local data directory is required.",
                    nameof(localDataDirectory));
            }

            lock (Sync)
            {
                string dataDirectory = Path.GetFullPath(localDataDirectory);
                Directory.CreateDirectory(dataDirectory);
                settingsStore = new ProviderSettingsStore(dataDirectory);
                cache = new MetadataCache(dataDirectory);
                (transport as IDisposable)?.Dispose();
                transport = new MetadataTransport();
                RebuildProviders();
            }
        }

        public static MetadataProviderStatus GetStatus()
        {
            EnsureInitialized();
            ProviderSettings settings = settingsStore.Load();
            return new MetadataProviderStatus
            {
                TmdbConfigured = !string.IsNullOrWhiteSpace(settings.TmdbAccessToken),
                IgdbConfigured = !string.IsNullOrWhiteSpace(settings.IgdbClientId)
                    && !string.IsNullOrWhiteSpace(settings.IgdbClientSecret),
                TmdbSource = settings.TmdbSource,
                IgdbSource = settings.IgdbSource
            };
        }

        public static void SaveSettings(
            string tmdbAccessToken,
            string igdbClientId,
            string igdbClientSecret)
        {
            EnsureInitialized();
            lock (Sync)
            {
                settingsStore.Save(tmdbAccessToken, igdbClientId, igdbClientSecret);
                RebuildProviders();
            }
        }

        public static async Task<IReadOnlyList<MetadataSearchResult>> SearchAsync(
            MetadataSearchRequest request,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
            {
                return new List<MetadataSearchResult>();
            }

            MetadataSearchResult manual = ManualResult(request);
            IMetadataProvider provider = ProviderFor(request.Kind);
            if (provider == null || !provider.IsConfigured)
            {
                return new[] { manual };
            }

            string cacheKey =
                $"search|{provider.Name}|{request.Kind}|{request.Query.Trim().ToLowerInvariant()}|{request.Limit}";
            List<MetadataSearchResult> cached;
            if (cache.TryRead(cacheKey, SearchLifetime, false, out cached))
            {
                cached.Add(manual);
                return cached;
            }

            try
            {
                IReadOnlyList<MetadataSearchResult> results =
                    await ExecuteWithTimeout(
                        token => provider.SearchAsync(request, token),
                        cancellationToken).ConfigureAwait(false);
                List<MetadataSearchResult> list = results?.ToList()
                    ?? new List<MetadataSearchResult>();
                cache.Write(cacheKey, provider.Name, list);
                list.Add(manual);
                return list;
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                if (cache.TryRead(cacheKey, SearchLifetime, true, out cached))
                {
                    cached.Add(manual);
                    return cached;
                }

                return new[] { manual };
            }
        }

        public static async Task<MediaMetadata> GetDetailsAsync(
            MediaType kind,
            string reference,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            MetadataReference parsed = MetadataReference.Parse(reference, kind);
            if (parsed.IsManual)
            {
                return new MediaMetadata
                {
                    ProviderName = "Manual",
                    ProviderId = parsed.ProviderId,
                    Kind = kind,
                    RetrievedAtUtc = DateTime.UtcNow,
                    Name = parsed.ProviderId,
                    ExternalUrl = reference
                };
            }

            IMetadataProvider provider = providers.FirstOrDefault(
                item => string.Equals(
                    item.Name,
                    parsed.ProviderName,
                    StringComparison.OrdinalIgnoreCase));
            if (provider == null || !provider.IsConfigured)
            {
                return null;
            }

            string cacheKey =
                $"details|{provider.Name}|{kind}|{parsed.ProviderId.ToLowerInvariant()}";
            MediaMetadata cached;
            if (cache.TryRead(cacheKey, DetailLifetime, false, out cached))
            {
                return cached;
            }

            try
            {
                MediaMetadata result = await ExecuteWithTimeout(
                    token => provider.GetDetailsAsync(parsed.ProviderId, kind, token),
                    cancellationToken).ConfigureAwait(false);
                if (result != null)
                {
                    result.RetrievedAtUtc = DateTime.UtcNow;
                    cache.Write(cacheKey, provider.Name, result);
                }

                return result;
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                if (cache.TryRead(cacheKey, DetailLifetime, true, out cached))
                {
                    return cached;
                }

                return null;
            }
        }

        public static string SeasonReference(string showReference, int seasonNumber)
        {
            MetadataReference parsed = MetadataReference.Parse(
                showReference,
                MediaType.TVShows);
            if (parsed.IsManual)
            {
                return MetadataReference.Manual(
                    MediaType.Seasons,
                    $"Season {seasonNumber}");
            }

            if (parsed.ProviderId.StartsWith("imdb:", StringComparison.OrdinalIgnoreCase))
            {
                return MetadataReference.Provider(
                    "TMDB",
                    MediaType.Seasons,
                    $"{parsed.ProviderId}/season/{seasonNumber}");
            }

            return $"https://www.themoviedb.org/tv/{parsed.ProviderId}/season/{seasonNumber}";
        }

        public static string EpisodeReference(
            string seasonReference,
            int episodeNumber)
        {
            MetadataReference parsed = MetadataReference.Parse(
                seasonReference,
                MediaType.Seasons);
            if (parsed.IsManual)
            {
                return MetadataReference.Manual(
                    MediaType.Episodes,
                    $"Episode {episodeNumber}");
            }

            if (parsed.ProviderId.StartsWith("imdb:", StringComparison.OrdinalIgnoreCase))
            {
                return MetadataReference.Provider(
                    "TMDB",
                    MediaType.Episodes,
                    $"{parsed.ProviderId}/episode/{episodeNumber}");
            }

            return $"https://www.themoviedb.org/tv/{parsed.ProviderId}/episode/{episodeNumber}";
        }

        private static async Task<T> ExecuteWithTimeout<T>(
            Func<CancellationToken, Task<T>> action,
            CancellationToken cancellationToken)
        {
            using (CancellationTokenSource timeout =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(TimeSpan.FromSeconds(12));
                try
                {
                    return await action(timeout.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new MetadataProviderException(
                        "The metadata provider timed out. Manual entry remains available.");
                }
            }
        }

        private static IMetadataProvider ProviderFor(MediaType kind)
        {
            return providers.FirstOrDefault(
                provider => provider.Supports(kind) && provider.IsConfigured)
                ?? providers.FirstOrDefault(provider => provider.Supports(kind));
        }

        private static void RebuildProviders()
        {
            ProviderSettings settings = settingsStore.Load();
            providers = new List<IMetadataProvider>
            {
                new TmdbMetadataProvider(settings.TmdbAccessToken, transport),
                new IgdbMetadataProvider(
                    settings.IgdbClientId,
                    settings.IgdbClientSecret,
                    transport)
            };
        }

        private static MetadataSearchResult ManualResult(
            MetadataSearchRequest request)
        {
            string name = request.Query.Trim();
            return new MetadataSearchResult
            {
                ProviderName = "Manual",
                ProviderId = name,
                Kind = request.Kind,
                Name = $"{name} (manual)",
                ExternalUrl = MetadataReference.Manual(request.Kind, name),
                Type = request.Kind == MediaType.Games ? "Game" : string.Empty
            };
        }

        private static void EnsureInitialized()
        {
            if (providers != null)
            {
                return;
            }

            string fallback = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Media_Manager");
            Initialize(fallback);
        }
    }

    internal sealed class MetadataReference
    {
        public string ProviderName { get; private set; }
        public string ProviderId { get; private set; }
        public bool IsManual { get; private set; }

        public static MetadataReference Parse(string value, MediaType kind)
        {
            value = value ?? string.Empty;
            Uri uri;
            if (Uri.TryCreate(value, UriKind.Absolute, out uri)
                && string.Equals(uri.Scheme, "metadata", StringComparison.OrdinalIgnoreCase))
            {
                string id = Uri.UnescapeDataString(
                    uri.Segments.LastOrDefault()?.Trim('/') ?? string.Empty);
                bool manual = string.Equals(
                    uri.Host,
                    "manual",
                    StringComparison.OrdinalIgnoreCase);
                return new MetadataReference
                {
                    ProviderName = manual ? "Manual" : uri.Host.ToUpperInvariant(),
                    ProviderId = id,
                    IsManual = manual
                };
            }

            if (value.IndexOf("themoviedb.org", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string path = new Uri(value).AbsolutePath.Trim('/');
                string[] segments = path.Split('/');
                string id = string.Join(
                    "/",
                    segments.Skip(1).Where(segment => !string.IsNullOrWhiteSpace(segment)));
                return new MetadataReference
                {
                    ProviderName = "TMDB",
                    ProviderId = id
                };
            }

            if (value.IndexOf("igdb.com/games/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string slug = new Uri(value).AbsolutePath
                    .Trim('/')
                    .Split('/')
                    .LastOrDefault();
                return new MetadataReference
                {
                    ProviderName = "IGDB",
                    ProviderId = slug ?? string.Empty
                };
            }

            Match imdb = Regex.Match(
                value,
                @"imdb\.com/title/(?<id>tt\d+)",
                RegexOptions.IgnoreCase);
            if (imdb.Success)
            {
                return new MetadataReference
                {
                    ProviderName = "TMDB",
                    ProviderId = "imdb:" + imdb.Groups["id"].Value
                };
            }

            return new MetadataReference
            {
                ProviderName = "Manual",
                ProviderId = string.Empty,
                IsManual = true
            };
        }

        public static string Manual(MediaType kind, string name)
        {
            return $"metadata://manual/{kind}/{Uri.EscapeDataString(name ?? string.Empty)}";
        }

        public static string Provider(
            string provider,
            MediaType kind,
            string providerId)
        {
            return $"metadata://{provider.ToLowerInvariant()}/{kind}/{Uri.EscapeDataString(providerId ?? string.Empty)}";
        }
    }
}
