using System;
using System.Collections.Generic;

namespace Media_Manager.Metadata
{
    public sealed class MetadataSearchRequest
    {
        public MediaType Kind { get; set; }
        public string Query { get; set; }
        public int Limit { get; set; } = 20;
    }

    public sealed class MetadataSearchResult
    {
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public MediaType Kind { get; set; }
        public string Name { get; set; }
        public string ArtworkUrl { get; set; }
        public string ExternalUrl { get; set; }
        public string Subtitle { get; set; }
        public string Type { get; set; }
        public List<string> Platforms { get; set; } = new List<string>();
    }

    public sealed class MediaMetadata
    {
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public MediaType Kind { get; set; }
        public DateTime RetrievedAtUtc { get; set; }
        public string Name { get; set; }
        public string ArtworkUrl { get; set; }
        public string ExternalUrl { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleasePeriod { get; set; }
        public string Region { get; set; }
        public string AgeRating { get; set; }
        public string Publisher { get; set; }
        public string Type { get; set; }
        public float UserScore { get; set; }
        public int UserReviewCount { get; set; }
        public float CriticScore { get; set; }
        public int CriticReviewCount { get; set; }
        public int SeasonCount { get; set; }
        public int EpisodeCount { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Stars { get; set; } = new List<string>();
        public List<string> Directors { get; set; } = new List<string>();
        public List<string> Writers { get; set; } = new List<string>();
        public List<string> Creators { get; set; } = new List<string>();
        public List<string> ProductionCompanies { get; set; } = new List<string>();
        public List<string> Platforms { get; set; } = new List<string>();
    }

    public sealed class MetadataProviderStatus
    {
        public bool TmdbConfigured { get; set; }
        public bool IgdbConfigured { get; set; }
        public string TmdbSource { get; set; }
        public string IgdbSource { get; set; }
    }

    public sealed class MetadataProviderException : Exception
    {
        public MetadataProviderException(string message) : base(message)
        {
        }

        public MetadataProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
