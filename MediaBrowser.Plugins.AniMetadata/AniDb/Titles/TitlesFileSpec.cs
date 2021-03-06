﻿using System.IO;
using MediaBrowser.Plugins.AniMetadata.Files;

namespace MediaBrowser.Plugins.AniMetadata.AniDb.Titles
{
    internal class TitlesFileSpec : IRemoteFileSpec<TitleListData>
    {
        private const string TitlesPath = "anidb\\titles";
        private readonly string _rootPath;

        public TitlesFileSpec(string rootPath)
        {
            _rootPath = rootPath;
        }

        public string Url => "http://anidb.net/api/animetitles.xml";

        public string LocalPath => Path.Combine(_rootPath, TitlesPath, "titles.xml");

        public bool IsGZipped => true;
    }
}