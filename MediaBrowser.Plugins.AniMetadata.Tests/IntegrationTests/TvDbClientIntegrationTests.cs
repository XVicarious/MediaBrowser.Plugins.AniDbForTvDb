﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Plugins.AniMetadata.Configuration;
using MediaBrowser.Plugins.AniMetadata.Files;
using MediaBrowser.Plugins.AniMetadata.Tests.TestHelpers;
using MediaBrowser.Plugins.AniMetadata.TvDb;
using MediaBrowser.Plugins.AniMetadata.TvDb.Data;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace MediaBrowser.Plugins.AniMetadata.Tests.IntegrationTests
{
    [TestFixture]
    [Explicit]
    public class TvDbClientIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            _logManager = new ConsoleLogManager();
            _applicationPaths = Substitute.For<IApplicationPaths>();
            _fileCache = Substitute.For<IFileCache>();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new OptionJsonConverter() }
            };
        }

        private ConsoleLogManager _logManager;
        private IApplicationPaths _applicationPaths;
        private IFileCache _fileCache;

        [Test]
        public async Task FindSeriesAsync_MatchingSeriesName_ReturnsSeries()
        {
            var client = new TvDbClient(new TvDbConnection(new TestHttpClient(), new JsonSerialiser(), _logManager),
                _fileCache, _applicationPaths, _logManager, new JsonSerialiser(), new PluginConfiguration
                {
                    TvDbApiKey = Secrets.Instance.TvDbApiKey
                });

            var seriesResult = await client.FindSeriesAsync("Mobile Suit Gundam 00");

            seriesResult.IsSome.Should().BeTrue();

            var series = seriesResult.ValueUnsafe();

            series.ShouldBeEquivalentTo(new TvDbSeriesData(80675, "Mobile Suit Gundam 00",
                new DateTime(2007, 10, 6), "Tokyo Broadcasting System", 30,
                DayOfWeek.Saturday, "6:00 PM", 9.4f, new string[] { },
                new[] { "Animation", "Drama", "Science-Fiction" },
                "2307 AD.\r\nAs fossil fuels became exhausted, humanity found a new energy source to change their situation: A large-scale solar power system with three enormous orbiting elevators. However, only a few large countries and their allies reaped the benefits.\r\nThree superpowers had ownership of the three orbiting elevators: The Union, based in the United States Of America, The People`s Reform League, made up of China, Russia, and India, and Europe`s AEU. The superpowers continue playing a large zero-sum game for their own dignity and respective prosperity. Even though it is the 24th century, humanity has yet to become one.\r\nIn this world where war has no end, a private militia appears advocating the eradication of war through force. Each possessing a mobile suit Gundam, they are Celestial Being. The armed intervention by the Gundams against all acts of war begins."));
        }

        [Test]
        public async Task FindSeriesAsync_NoMatchingSeriesName_ReturnsNone()
        {
            var client = new TvDbClient(new TvDbConnection(new TestHttpClient(), new JsonSerialiser(), _logManager),
                _fileCache, _applicationPaths, _logManager, new JsonSerialiser(), new PluginConfiguration
                {
                    TvDbApiKey = Secrets.Instance.TvDbApiKey
                });

            var seriesResult = await client.FindSeriesAsync("NotASeries");

            seriesResult.IsSome.Should().BeFalse();
        }

        [Test]
        public async Task GetEpisodesAsync_ValidSeriesId_ReturnsEpisodes()
        {
            var client = new TvDbClient(new TvDbConnection(new TestHttpClient(), new JsonSerialiser(), _logManager),
                _fileCache, _applicationPaths, _logManager, new JsonSerialiser(), new PluginConfiguration
                {
                    TvDbApiKey = Secrets.Instance.TvDbApiKey
                });

            var episodesResult = await client.GetEpisodesAsync(80675);

            episodesResult.IsSome.Should().BeTrue();
            var episodes = episodesResult.ValueUnsafe().ToList();

            episodes.Should().HaveCount(57);

            episodes[0]
                .ShouldBeEquivalentTo(new TvDbEpisodeData(340368, "Celestial Being", 1L, 1, 1, 1496255818,
                    new DateTime(2007, 10, 6),
                    "Celestial Being, a private army dedicated to eradicating war, begins demonstrating the powers of their new \"MS-GUNDAM\" suits by interrupting the public demonstration of AEU's latest Mobile Suit, the AEU Enact and by protecting the Human Reform League's Space Elevator, \"Tenchu\" from being attacked by terrorists when their mobile suits had attempted to launch rockets on the \"Tenchu\", earning a news appearance from various TV news channels where Celestial Being's goals were publicly stated by Aeoria Schenberg.",
                    8.2f, 5));
        }

        [Test]
        public async Task GetSeriesAsync_ValidSeriesId_ReturnsSeriesData()
        {
            var client = new TvDbClient(new TvDbConnection(new TestHttpClient(), new JsonSerialiser(), _logManager),
                _fileCache, _applicationPaths, _logManager, new JsonSerialiser(), new PluginConfiguration
                {
                    TvDbApiKey = Secrets.Instance.TvDbApiKey
                });

            var seriesResult = await client.GetSeriesAsync(80675);

            seriesResult.IsSome.Should().BeTrue();

            var series = seriesResult.ValueUnsafe();

            series.ShouldBeEquivalentTo(new TvDbSeriesData(80675, "Mobile Suit Gundam 00",
                new DateTime(2007, 10, 6), "Tokyo Broadcasting System", 30, DayOfWeek.Saturday, "6:00 PM",
                9.4f, new string[] { }, new[] { "Animation", "Drama", "Science-Fiction" },
                "2307 AD.\r\nAs fossil fuels became exhausted, humanity found a new energy source to change their situation: A large-scale solar power system with three enormous orbiting elevators. However, only a few large countries and their allies reaped the benefits.\r\nThree superpowers had ownership of the three orbiting elevators: The Union, based in the United States Of America, The People`s Reform League, made up of China, Russia, and India, and Europe`s AEU. The superpowers continue playing a large zero-sum game for their own dignity and respective prosperity. Even though it is the 24th century, humanity has yet to become one.\r\nIn this world where war has no end, a private militia appears advocating the eradication of war through force. Each possessing a mobile suit Gundam, they are Celestial Being. The armed intervention by the Gundams against all acts of war begins."));
        }
    }
}