﻿using System;
using LanguageExt;

namespace MediaBrowser.Plugins.AniMetadata.Process
{
    internal class IdentifierOnlySourceData : ISourceData<IdentifierOnlySourceData>
    {
        public IdentifierOnlySourceData(ISource source, Option<int> id, IItemIdentifier identifier)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Id = id;
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        public ISource Source { get; }

        public Option<int> Id { get; }

        public IItemIdentifier Identifier { get; }

        IdentifierOnlySourceData ISourceData<IdentifierOnlySourceData>.Data => this;

        public object Data => this;

        public Option<TData> GetData<TData>() where TData : class
        {
            return this as TData;
        }
    }
}