﻿using System.Collections.Generic;
using Functional.Maybe;
using MediaBrowser.Plugins.Anime.AniDb.Series.Data;

namespace MediaBrowser.Plugins.Anime.Providers.AniDb2
{
    internal interface IEpisodeMatcher
    {
        /// <summary>
        ///     Finds an episode in the collection that best matches the criteria
        /// </summary>
        Maybe<EpisodeData> FindEpisode(IEnumerable<EpisodeData> episodes, Maybe<int> seasonIndex,
            Maybe<int> episodeIndex, Maybe<string> title);
    }
}