using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Media_Manager.Metadata
{
    public interface IMetadataProvider
    {
        string Name { get; }
        bool IsConfigured { get; }
        bool Supports(MediaType kind);
        Task<IReadOnlyList<MetadataSearchResult>> SearchAsync(
            MetadataSearchRequest request,
            CancellationToken cancellationToken);
        Task<MediaMetadata> GetDetailsAsync(
            string providerId,
            MediaType kind,
            CancellationToken cancellationToken);
    }
}
