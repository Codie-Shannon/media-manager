using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Media_Manager.Metadata
{
    public sealed class TmdbMetadataProvider : IMetadataProvider
    {
        private const string ApiBase = "https://api.themoviedb.org/3/";
        private const string SiteBase = "https://www.themoviedb.org/";
        private const string ImageBase = "https://image.tmdb.org/t/p/w500";
        private readonly string accessToken;
        private readonly IMetadataTransport transport;

        public TmdbMetadataProvider(string accessToken, IMetadataTransport transport)
        {
            this.accessToken = accessToken?.Trim();
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public string Name => "TMDB";

        public bool IsConfigured => !string.IsNullOrWhiteSpace(accessToken);

        public bool Supports(MediaType kind)
        {
            return kind == MediaType.Movies
                || kind == MediaType.TVShows
                || kind == MediaType.Seasons
                || kind == MediaType.Episodes;
        }

        public async Task<IReadOnlyList<MetadataSearchResult>> SearchAsync(
            MetadataSearchRequest request,
            CancellationToken cancellationToken)
        {
            EnsureConfigured();
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
            {
                return new List<MetadataSearchResult>();
            }

            string route = request.Kind == MediaType.Movies ? "movie" : "tv";
            string url = $"{ApiBase}search/{route}?query={Uri.EscapeDataString(request.Query.Trim())}&include_adult=false&language=en-NZ&page=1";
            JObject root = JObject.Parse(await GetAsync(url, cancellationToken).ConfigureAwait(false));
            List<MetadataSearchResult> results = new List<MetadataSearchResult>();
            foreach (JToken item in root["results"] ?? new JArray())
            {
                string id = Value(item, "id");
                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                string name = Value(item, request.Kind == MediaType.Movies ? "title" : "name");
                string date = Value(
                    item,
                    request.Kind == MediaType.Movies ? "release_date" : "first_air_date");
                results.Add(new MetadataSearchResult
                {
                    ProviderName = Name,
                    ProviderId = id,
                    Kind = request.Kind,
                    Name = name,
                    ArtworkUrl = Artwork(Value(item, "poster_path")),
                    ExternalUrl = $"{SiteBase}{route}/{id}",
                    Subtitle = Year(date)
                });

                if (results.Count >= Math.Max(1, request.Limit))
                {
                    break;
                }
            }

            return results;
        }

        public async Task<MediaMetadata> GetDetailsAsync(
            string providerId,
            MediaType kind,
            CancellationToken cancellationToken)
        {
            EnsureConfigured();
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return null;
            }

            if (providerId.StartsWith("imdb:", StringComparison.OrdinalIgnoreCase))
            {
                providerId = await ResolveImdbReferenceAsync(
                    providerId,
                    kind,
                    cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(providerId))
                {
                    return null;
                }
            }

            string route = DetailRoute(providerId, kind);
            string append = kind == MediaType.Movies
                ? "credits,release_dates"
                : kind == MediaType.TVShows
                    ? "credits,content_ratings"
                    : "credits";
            string url = $"{ApiBase}{route}?append_to_response={append}&language=en-NZ";
            JObject root = JObject.Parse(await GetAsync(url, cancellationToken).ConfigureAwait(false));
            MediaMetadata metadata = new MediaMetadata
            {
                ProviderName = Name,
                ProviderId = providerId,
                Kind = kind,
                RetrievedAtUtc = DateTime.UtcNow,
                Name = Value(root, kind == MediaType.Movies ? "title" : "name"),
                ArtworkUrl = Artwork(
                    Value(
                        root,
                        kind == MediaType.Episodes ? "still_path" : "poster_path")),
                ExternalUrl = SiteUrl(providerId, kind),
                ReleaseDate = Value(
                    root,
                    kind == MediaType.Movies
                        ? "release_date"
                        : kind == MediaType.Episodes
                            ? "air_date"
                            : "first_air_date"),
                UserScore = Float(root["vote_average"]),
                UserReviewCount = Integer(root["vote_count"]),
                Genres = Names(root["genres"]),
                ProductionCompanies = Names(root["production_companies"])
            };

            JObject credits = root["credits"] as JObject;
            metadata.Stars = Names(credits?["cast"], 10);
            metadata.Directors = Crew(credits, "Director");
            metadata.Writers = Crew(credits, "Writer", "Screenplay", "Teleplay");

            if (kind == MediaType.Movies)
            {
                metadata.AgeRating = MovieCertification(root["release_dates"]);
                metadata.Region = "NZ";
            }
            else if (kind == MediaType.TVShows)
            {
                metadata.Creators = Names(root["created_by"]);
                metadata.AgeRating = TvCertification(root["content_ratings"]);
                metadata.SeasonCount = Integer(root["number_of_seasons"]);
                metadata.EpisodeCount = Integer(root["number_of_episodes"]);
                string first = Value(root, "first_air_date");
                string last = Value(root, "last_air_date");
                metadata.ReleasePeriod = string.IsNullOrWhiteSpace(last)
                    ? Year(first)
                    : $"{Year(first)} - {Year(last)}";
                metadata.Region = Value(root, "origin_country", 0);
            }
            else if (kind == MediaType.Seasons)
            {
                metadata.EpisodeCount = (root["episodes"] as JArray)?.Count ?? 0;
            }

            return metadata;
        }

        private async Task<string> ResolveImdbReferenceAsync(
            string providerId,
            MediaType kind,
            CancellationToken cancellationToken)
        {
            string[] parts = providerId.Split('/');
            string imdbId = parts[0].Substring("imdb:".Length);
            string url =
                $"{ApiBase}find/{Uri.EscapeDataString(imdbId)}?external_source=imdb_id&language=en-NZ";
            JObject root = JObject.Parse(
                await GetAsync(url, cancellationToken).ConfigureAwait(false));
            string collection = kind == MediaType.Movies
                ? "movie_results"
                : "tv_results";
            string tmdbId = Value(root[collection]?.FirstOrDefault(), "id");
            if (string.IsNullOrWhiteSpace(tmdbId))
            {
                return string.Empty;
            }

            return parts.Length == 1
                ? tmdbId
                : tmdbId + "/" + string.Join("/", parts.Skip(1));
        }

        private async Task<string> GetAsync(string url, CancellationToken cancellationToken)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.ParseAdd("application/json");
                return await transport.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private void EnsureConfigured()
        {
            if (!IsConfigured)
            {
                throw new MetadataProviderException(
                    "TMDB is not configured. Add its read access token in Settings or use manual entry.");
            }
        }

        private static string DetailRoute(string providerId, MediaType kind)
        {
            if (kind == MediaType.Movies)
            {
                return "movie/" + providerId;
            }

            if (kind == MediaType.TVShows)
            {
                return "tv/" + providerId;
            }

            string normalized = providerId.Trim('/');
            return "tv/" + normalized;
        }

        private static string SiteUrl(string providerId, MediaType kind)
        {
            if (kind == MediaType.Movies)
            {
                return $"{SiteBase}movie/{providerId}";
            }

            return $"{SiteBase}tv/{providerId.Trim('/')}";
        }

        private static string Artwork(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? string.Empty : ImageBase + path;
        }

        private static string Value(JToken token, string name)
        {
            return token?[name]?.Type == JTokenType.Null ? string.Empty : $"{token?[name]}";
        }

        private static string Value(JToken token, string name, int index)
        {
            return $"{token?[name]?[index]}";
        }

        private static int Integer(JToken token)
        {
            return token == null || token.Type == JTokenType.Null ? 0 : token.Value<int>();
        }

        private static float Float(JToken token)
        {
            return token == null || token.Type == JTokenType.Null
                ? 0
                : token.Value<float>();
        }

        private static string Year(string date)
        {
            DateTime parsed;
            return DateTime.TryParse(date, out parsed)
                ? parsed.Year.ToString()
                : string.Empty;
        }

        private static List<string> Names(JToken collection, int limit = int.MaxValue)
        {
            return (collection as JArray ?? new JArray())
                .Select(item => Value(item, "name"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(limit)
                .ToList();
        }

        private static List<string> Crew(JObject credits, params string[] jobs)
        {
            HashSet<string> accepted = new HashSet<string>(
                jobs,
                StringComparer.OrdinalIgnoreCase);
            return (credits?["crew"] as JArray ?? new JArray())
                .Where(item => accepted.Contains(Value(item, "job")))
                .Select(item => Value(item, "name"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string MovieCertification(JToken releaseDates)
        {
            return Certification(releaseDates?["results"], "release_dates", "certification");
        }

        private static string TvCertification(JToken contentRatings)
        {
            return Certification(contentRatings?["results"], null, "rating");
        }

        private static string Certification(
            JToken resultsToken,
            string nestedCollection,
            string valueName)
        {
            JArray results = resultsToken as JArray ?? new JArray();
            foreach (string region in new[] { "NZ", "AU", "US" })
            {
                JToken result = results.FirstOrDefault(
                    item => string.Equals(Value(item, "iso_3166_1"), region, StringComparison.OrdinalIgnoreCase));
                IEnumerable<JToken> values;
                if (nestedCollection == null)
                {
                    values = new[] { result };
                }
                else
                {
                    values = result?[nestedCollection] as JArray ?? new JArray();
                }
                string certification = values
                    .Select(item => Value(item, valueName))
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
                if (!string.IsNullOrWhiteSpace(certification))
                {
                    return certification;
                }
            }

            return string.Empty;
        }
    }
}
