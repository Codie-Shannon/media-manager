using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Media_Manager.Metadata
{
    public interface IMetadataTransport
    {
        Task<string> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }

    public sealed class MetadataTransport : IMetadataTransport, IDisposable
    {
        private readonly HttpClient client;

        public MetadataTransport()
        {
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MediaManager/1.0");
        }

        public async Task<string> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException exception)
            {
                throw new MetadataProviderException(
                    "The metadata provider could not be reached. Manual entry remains available.",
                    exception);
            }

            using (response)
            {
                string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Unauthorized
                    || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new MetadataProviderException(
                        "The metadata provider rejected the saved credentials. Update them in Settings.");
                }

                if ((int)response.StatusCode == 429)
                {
                    throw new MetadataProviderException(
                        "The metadata provider rate limit was reached. Try again shortly or use manual entry.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new MetadataProviderException(
                        $"The metadata provider returned HTTP {(int)response.StatusCode}. Manual entry remains available.");
                }

                return body;
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
