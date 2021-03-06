﻿using System.Threading.Tasks;
using LanguageExt;
using MediaBrowser.Plugins.AniMetadata.AniDb.SeriesData;
using MediaBrowser.Plugins.AniMetadata.Process;

namespace MediaBrowser.Plugins.AniMetadata.SourceDataLoaders
{
    /// <summary>
    ///     Loads series data for an item that already has AniDb episode data loaded
    /// </summary>
    internal class AniDbSeriesFromAniDbEpisode : ISourceDataLoader
    {
        private readonly ISources _sources;

        public AniDbSeriesFromAniDbEpisode(ISources sources)
        {
            _sources = sources;
        }

        public bool CanLoadFrom(object sourceData)
        {
            return sourceData is ISourceData<AniDbEpisodeData>;
        }

        public Task<Either<ProcessFailedResult, ISourceData>> LoadFrom(IMediaItem mediaItem, object sourceData)
        {
            var resultContext = new ProcessResultContext(nameof(AniDbSeriesFromAniDbEpisode),
                mediaItem.EmbyData.Identifier.Name,
                mediaItem.ItemType);

            return _sources.AniDb.GetSeriesData(mediaItem.EmbyData, resultContext)
                .BindAsync(s =>
                {
                    var title = _sources.AniDb.SelectTitle(s.Titles, mediaItem.EmbyData.Language, resultContext);

                    return title.Map(t => CreateSourceData(s, mediaItem.EmbyData, t));
                });
        }

        private ISourceData CreateSourceData(AniDbSeriesData seriesData, IEmbyItemData embyItemData, string title)
        {
            return new SourceData<AniDbSeriesData>(_sources.AniDb.ForAdditionalData(), seriesData.Id,
                new ItemIdentifier(embyItemData.Identifier.Index, Option<int>.None, title), seriesData);
        }
    }
}