﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using MediaBrowser.Plugins.AniMetadata.TvDb.Data;

namespace MediaBrowser.Plugins.AniMetadata.TvDb
{
    public interface ITvDbClient
    {
        Task<Option<TvDbSeriesData>> GetSeriesAsync(int tvDbSeriesId);

        Task<Option<TvDbSeriesData>> FindSeriesAsync(string seriesName);

        Task<Option<List<TvDbEpisodeData>>> GetEpisodesAsync(int tvDbSeriesId);

        Task<Option<TvDbEpisodeData>> GetEpisodeAsync(int tvDbSeriesId, int seasonIndex,
            int episodeIndex);

        Task<Option<TvDbEpisodeData>> GetEpisodeAsync(int tvDbSeriesId, int absoluteEpisodeIndex);
    }
}