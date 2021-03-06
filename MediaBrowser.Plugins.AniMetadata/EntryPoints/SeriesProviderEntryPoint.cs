﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using MediaBrowser.Plugins.AniMetadata.Process.Providers;

namespace MediaBrowser.Plugins.AniMetadata.EntryPoints
{
    public class SeriesProviderEntryPoint : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
    {
        private readonly SeriesProvider _seriesProvider;

        public SeriesProviderEntryPoint(IApplicationHost applicationHost)
        {
            _seriesProvider =
                DependencyConfiguration.Resolve<SeriesProvider>(applicationHost);
        }

        public int Order => _seriesProvider.Order;

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo,
            CancellationToken cancellationToken)
        {
            return _seriesProvider.GetSearchResults(searchInfo, cancellationToken);
        }

        public Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
        {
            return _seriesProvider.GetMetadata(info, cancellationToken);
        }

        public string Name => _seriesProvider.Name;

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _seriesProvider.GetImageResponse(url, cancellationToken);
        }
    }
}