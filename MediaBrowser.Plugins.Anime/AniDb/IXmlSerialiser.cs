﻿namespace MediaBrowser.Plugins.Anime.AniDb
{
    public interface IXmlSerialiser
    {
        T Deserialise<T>(string xml) where T : class;

        void SerialiseToFile<T>(string filePath, T data) where T : class;
    }
}