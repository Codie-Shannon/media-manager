using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Media_Manager.Metadata
{
    public sealed class IgdbMetadataProvider : IMetadataProvider
    {
        private const string GamesUrl = "https://api.igdb.com/v4/games";
        private const string TokenUrl = "https://id.twitch.tv/oauth2/token";
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly IMetadataTransport transport;
        private string accessToken;
        private DateTime tokenExpiryUtc;

        public IgdbMetadataProvider(
            string clientId,
            string clientSecret,
            IMetadataTransport transport)
        {
            this.clientId = clientId?.Trim();
            this.clientSecret = clientSecret?.Trim();
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public string Name => "IGDB";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(clientId)
            && !string.IsNullOrWhiteSpace(clientSecret);

        public bool Supports(MediaType kind)
        {
            return kind == MediaType.Games;
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

            string fields = Fields();
            string body =
                $"search \"{Escape(request.Query.Trim())}\"; fields {fields}; where version_parent = null; limit {Math.Max(1, request.Limit)};";
            JArray root = JArray.Parse(
                await SendGamesAsync(body, cancellationToken).ConfigureAwait(false));
            return root.Select(ToSearchResult).Where(result => result != null).ToList();
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

            string body =
                $"fields {Fields()}; where slug = \"{Escape(providerId)}\"; limit 1;";
            JToken game = JArray.Parse(
                await SendGamesAsync(body, cancellationToken).ConfigureAwait(false))
                .FirstOrDefault();
            if (game == null)
            {
                return null;
            }

            JToken publisher = (game["involved_companies"] as JArray ?? new JArray())
                .FirstOrDefault(item => item["publisher"]?.Value<bool>() == true);
            return new MediaMetadata
            {
                ProviderName = Name,
                ProviderId = providerId,
                Kind = MediaType.Games,
                RetrievedAtUtc = DateTime.UtcNow,
                Name = Value(game, "name"),
                ArtworkUrl = Cover(game),
                ExternalUrl = "https://www.igdb.com/games/" + Value(game, "slug"),
                ReleaseDate = UnixDate(game["first_release_date"]),
                Publisher = Value(publisher?["company"], "name"),
                Type = Category(game["category"]),
                UserScore = Float(game["rating"]),
                UserReviewCount = Integer(game["rating_count"]),
                CriticScore = Float(game["aggregated_rating"]),
                CriticReviewCount = Integer(game["aggregated_rating_count"]),
                Genres = Names(game["genres"]),
                Platforms = Names(game["platforms"])
            };
        }

        private async Task<string> SendGamesAsync(
            string body,
            CancellationToken cancellationToken)
        {
            string token = await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GamesUrl))
            {
                request.Headers.Add("Client-ID", clientId);
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.ParseAdd("application/json");
                request.Content = new StringContent(body, Encoding.UTF8, "text/plain");
                return await transport.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken)
                && tokenExpiryUtc > DateTime.UtcNow.AddMinutes(1))
            {
                return accessToken;
            }

            string url =
                $"{TokenUrl}?client_id={Uri.EscapeDataString(clientId)}&client_secret={Uri.EscapeDataString(clientSecret)}&grant_type=client_credentials";
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                JObject response = JObject.Parse(
                    await transport.SendAsync(request, cancellationToken).ConfigureAwait(false));
                accessToken = Value(response, "access_token");
                int expiresIn = Integer(response["expires_in"]);
                tokenExpiryUtc = DateTime.UtcNow.AddSeconds(Math.Max(60, expiresIn));
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new MetadataProviderException(
                        "IGDB authentication did not return an access token.");
                }

                return accessToken;
            }
        }

        private MetadataSearchResult ToSearchResult(JToken game)
        {
            string slug = Value(game, "slug");
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            return new MetadataSearchResult
            {
                ProviderName = Name,
                ProviderId = slug,
                Kind = MediaType.Games,
                Name = Value(game, "name"),
                ArtworkUrl = Cover(game),
                ExternalUrl = "https://www.igdb.com/games/" + slug,
                Subtitle = UnixYear(game["first_release_date"]),
                Type = Category(game["category"]),
                Platforms = Names(game["platforms"])
            };
        }

        private void EnsureConfigured()
        {
            if (!IsConfigured)
            {
                throw new MetadataProviderException(
                    "IGDB is not configured. Add its client credentials in Settings or use manual entry.");
            }
        }

        private static string Fields()
        {
            return "id,name,slug,category,cover.url,first_release_date,rating,rating_count,aggregated_rating,aggregated_rating_count,genres.name,platforms.name,involved_companies.publisher,involved_companies.company.name";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        private static string Value(JToken token, string name)
        {
            return token?[name]?.Type == JTokenType.Null ? string.Empty : $"{token?[name]}";
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

        private static List<string> Names(JToken collection)
        {
            return (collection as JArray ?? new JArray())
                .Select(item => Value(item, "name"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Cover(JToken game)
        {
            string cover = Value(game?["cover"], "url");
            if (string.IsNullOrWhiteSpace(cover))
            {
                return string.Empty;
            }

            cover = cover.Replace("t_thumb", "t_cover_big");
            return cover.StartsWith("//", StringComparison.Ordinal)
                ? "https:" + cover
                : cover;
        }

        private static string UnixDate(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return string.Empty;
            }

            DateTime date = DateTimeOffset
                .FromUnixTimeSeconds(token.Value<long>())
                .UtcDateTime;
            return date.ToString("yyyy-MM-dd");
        }

        private static string UnixYear(JToken token)
        {
            string date = UnixDate(token);
            return date.Length >= 4 ? date.Substring(0, 4) : string.Empty;
        }

        private static string Category(JToken token)
        {
            int category = Integer(token);
            switch (category)
            {
                case 0: return "Main Game";
                case 1: return "DLC";
                case 2: return "Expansion";
                case 3: return "Bundle";
                case 4: return "Standalone Expansion";
                case 5: return "Mod";
                case 8: return "Remake";
                case 9: return "Remaster";
                case 10: return "Expanded Game";
                case 11: return "Port";
                case 12: return "Fork";
                case 13: return "Pack";
                case 14: return "Update";
                default: return "Game";
            }
        }
    }
}
