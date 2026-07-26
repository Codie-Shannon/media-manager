using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MediaControlsLibrary;
using Media_Manager.Metadata;

namespace Media_Manager
{
    public static class SearchEngine
    {
        private static readonly object Sync = new object();
        private static CancellationTokenSource activeCancellation;
        private static Task activeSearch = Task.CompletedTask;

        public static async Task Abort(optSearchBox searchbox)
        {
            CancellationTokenSource cancellation;
            Task search;
            lock (Sync)
            {
                cancellation = activeCancellation;
                search = activeSearch;
                activeCancellation = null;
                activeSearch = Task.CompletedTask;
            }

            cancellation?.Cancel();
            try
            {
                await search;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                cancellation?.Dispose();
                if (searchbox != null)
                {
                    searchbox.isLoading = false;
                }
            }
        }

        public static void Stop()
        {
            CancellationTokenSource cancellation;
            lock (Sync)
            {
                cancellation = activeCancellation;
                activeCancellation = null;
                activeSearch = Task.CompletedTask;
            }

            cancellation?.Cancel();
            cancellation?.Dispose();
        }

        public static void VirtualEntertainmentSearch(
            optSearchBox searchbox,
            string search,
            MediaType mediatype = MediaType.Movies)
        {
            StartSearch(searchbox, search, mediatype);
        }

        public static void GameSearch(optSearchBox searchbox, string search)
        {
            StartSearch(searchbox, search, MediaType.Games);
        }

        private static void StartSearch(
            optSearchBox searchbox,
            string search,
            MediaType kind)
        {
            if (searchbox == null)
            {
                return;
            }

            CancellationTokenSource cancellation = new CancellationTokenSource();
            Task task;
            lock (Sync)
            {
                activeCancellation?.Cancel();
                activeCancellation = cancellation;
                task = RunSearchAsync(searchbox, search, kind, cancellation);
                activeSearch = task;
            }
        }

        private static async Task RunSearchAsync(
            optSearchBox searchbox,
            string search,
            MediaType kind,
            CancellationTokenSource cancellation)
        {
            searchbox.isLoading = true;
            searchbox.isError = false;

            try
            {
                IReadOnlyList<MetadataSearchResult> results =
                    await MetadataService.SearchAsync(
                        new MetadataSearchRequest
                        {
                            Kind = kind,
                            Query = search,
                            Limit = 20
                        },
                        cancellation.Token);
                cancellation.Token.ThrowIfCancellationRequested();

                searchbox.Clear(true);
                foreach (MetadataSearchResult result in results ?? Enumerable.Empty<MetadataSearchResult>())
                {
                    string cover = string.IsNullOrWhiteSpace(result.ArtworkUrl)
                        ? searchbox.DefaultCover?.ToString()
                        : result.ArtworkUrl;
                    if (kind == MediaType.Games)
                    {
                        searchbox.Add(
                            result.ExternalUrl,
                            result.Name,
                            cover,
                            result.Type,
                            result.Platforms ?? new List<string>());
                    }
                    else
                    {
                        searchbox.Add(
                            result.Name,
                            cover,
                            string.Empty,
                            result.ExternalUrl);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                searchbox.Clear(true);
                searchbox.SearchError =
                    $"Metadata search is unavailable: {exception.Message} Manual entry remains available.";
                searchbox.isError = true;
            }
            finally
            {
                bool ownsSearch;
                lock (Sync)
                {
                    ownsSearch = ReferenceEquals(activeCancellation, cancellation);
                    if (ownsSearch)
                    {
                        activeCancellation = null;
                        activeSearch = Task.CompletedTask;
                    }
                }

                if (ownsSearch)
                {
                    searchbox.isLoading = false;
                }

                cancellation.Dispose();
            }
        }
    }
}
