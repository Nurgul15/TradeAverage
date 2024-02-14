using Application.Services;
using Application.Trades.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace TradeAverageTests
{
    [TestOf(typeof(TradeService))]
    [TestFixture]
    public class TradeServiceTests
    {
        private Mock<IHttpClientFactory> _factory;
        private Mock<IMemoryCache> _memoryCache;
        private ITradeService _service;

        [SetUp]
        public void Init()
        {
            _factory = new Mock<IHttpClientFactory>();
            _memoryCache = new Mock<IMemoryCache>();
            _service = new TradeService(_factory.Object,
           _memoryCache.Object);
        }

        [Test]
        public void CalculateTradesAverage_ShouldReturnValidResult()
        {
            var date = DateTime.UtcNow;
            var recentTrade1 = new RecentTrade() { PrimaryCurrencyAmount = 0.145M, TradeTimestampUtc = date };
            var recentTrade2 = new RecentTrade() { PrimaryCurrencyAmount = 0.258M, TradeTimestampUtc = date.AddMinutes(1) };
            var recentTrade3 = new RecentTrade() { PrimaryCurrencyAmount = 0.532M, TradeTimestampUtc = date.AddMinutes(2) };
            var trades = new List<RecentTrade>() { recentTrade1, recentTrade2, recentTrade3 };

            var result = _service.CalculateTradesAverage(trades);

            Assert.That(result.AverageNumberPerMinute, Is.EqualTo(1));
            Assert.That(result.AverageVolumePerMinute, Is.EqualTo(0.47M));
        }

        [Test]
        public void CalculateTradesAverage_ShouldCatchException()
        {
            var tradeData = new RecentTradesResponse() { Trades = new List<RecentTrade>()};

            Assert.Throws<ArgumentNullException>(() => _service.CalculateTradesAverage(tradeData.Trades.ToList()));
        }
    }
}