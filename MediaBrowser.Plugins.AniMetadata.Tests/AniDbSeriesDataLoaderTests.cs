﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Logging;
using MediaBrowser.Plugins.AniMetadata.AniDb;
using MediaBrowser.Plugins.AniMetadata.AniDb.Mapping;
using MediaBrowser.Plugins.AniMetadata.AniDb.SeriesData;
using MediaBrowser.Plugins.AniMetadata.Providers.AniDb;
using MediaBrowser.Plugins.AniMetadata.Tests.TestHelpers;
using MediaBrowser.Plugins.AniMetadata.TvDb;
using MediaBrowser.Plugins.AniMetadata.TvDb.Data;
using NSubstitute;
using NUnit.Framework;

namespace MediaBrowser.Plugins.AniMetadata.Tests
{
    [TestFixture]
    public class AniDbSeriesDataLoaderTests
    {
        [SetUp]
        public void Setup()
        {
            _logManager = new ConsoleLogManager();
            _aniDbClient = Substitute.For<IAniDbClient>();
            _tvDbClient = Substitute.For<ITvDbClient>();
            _mapper = Substitute.For<IAniDbMapper>();
        }

        private ILogManager _logManager;
        private IAniDbClient _aniDbClient;
        private IAniDbMapper _mapper;
        private ITvDbClient _tvDbClient;

        [Test]
        public async Task GetSeriesDataAsync_NoAniDbSeriesData_ReturnsNullResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.IsT2.Should().BeTrue();
        }

        [Test]
        public async Task GetSeriesDataAsync_NoMapper_ReturnsAniDbResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var aniDbSeriesData = new AniDbSeriesData();

            _aniDbClient.FindSeriesAsync("AniDbTitle").Returns(aniDbSeriesData);

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.IsT0.Should().BeTrue();
            result.AsT0.AniDbSeriesData.Should().Be(aniDbSeriesData);
        }

        [Test]
        public async Task GetSeriesDataAsync_NoSeriesIds_ReturnsAniDbResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var aniDbSeriesData = new AniDbSeriesData();

            _aniDbClient.FindSeriesAsync("AniDbTitle").Returns(aniDbSeriesData);
            _aniDbClient.GetMapperAsync().Returns(Option<IAniDbMapper>.Some(_mapper));

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.IsT0.Should().BeTrue();
            result.AsT0.AniDbSeriesData.Should().Be(aniDbSeriesData);
        }

        [Test]
        public async Task GetSeriesDataAsync_NoTvDbSeriesData_ReturnsAniDbResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var seriesIds = new SeriesIds(1, 33, 2, 4);

            var aniDbSeriesData = new AniDbSeriesData
            {
                Id = 4
            };

            _aniDbClient.FindSeriesAsync("AniDbTitle").Returns(aniDbSeriesData);
            _aniDbClient.GetMapperAsync().Returns(Option<IAniDbMapper>.Some(_mapper));
            _mapper.GetMappedSeriesIdsFromAniDb(4).Returns(seriesIds);

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.AsT0.AniDbSeriesData.Should().Be(aniDbSeriesData);
        }

        [Test]
        public async Task GetSeriesDataAsync_NoTvDbSeriesId_ReturnsAniDbResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var seriesIds = new SeriesIds(1, Option<int>.None, 2, 4);

            var aniDbSeriesData = new AniDbSeriesData
            {
                Id = 4
            };

            _aniDbClient.FindSeriesAsync("AniDbTitle").Returns(aniDbSeriesData);
            _aniDbClient.GetMapperAsync().Returns(Option<IAniDbMapper>.Some(_mapper));
            _mapper.GetMappedSeriesIdsFromAniDb(4).Returns(seriesIds);

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.AsT0.AniDbSeriesData.Should().Be(aniDbSeriesData);
        }

        [Test]
        public async Task GetSeriesDataAsync_TvDbSeriesData_ReturnsCombinedResult()
        {
            var aniDbSeriesProvider =
                new AniDbSeriesDataLoader(_logManager, _aniDbClient, _tvDbClient);

            var seriesInfo = new SeriesInfo
            {
                Name = "AniDbTitle",
                MetadataLanguage = "en"
            };

            var seriesIds = new SeriesIds(1, 33, 2, 4);

            var aniDbSeriesData = new AniDbSeriesData
            {
                Id = 4
            };

            var tvDbSeriesData = new TvDbSeriesData(33, "Name", new List<string>(), new List<string>(), "Overview");

            _tvDbClient.GetSeriesAsync(33).Returns(tvDbSeriesData);

            _aniDbClient.FindSeriesAsync("AniDbTitle").Returns(aniDbSeriesData);
            _aniDbClient.GetMapperAsync().Returns(Option<IAniDbMapper>.Some(_mapper));
            _mapper.GetMappedSeriesIdsFromAniDb(4).Returns(seriesIds);

            var result = await aniDbSeriesProvider.GetSeriesDataAsync(seriesInfo);

            result.IsT1.Should().BeTrue();
            result.AsT1.AniDbSeriesData.Should().Be(aniDbSeriesData);
            result.AsT1.TvDbSeriesData.Should().Be(tvDbSeriesData);

            result.AsT1.SeriesIds.AniDbSeriesId.Should().Be(1);
            result.AsT1.SeriesIds.TvDbSeriesId.ValueUnsafe().Should().Be(33);
            result.AsT1.SeriesIds.ImdbSeriesId.ValueUnsafe().Should().Be(2);
            result.AsT1.SeriesIds.TmDbSeriesId.ValueUnsafe().Should().Be(4);
        }
    }
}