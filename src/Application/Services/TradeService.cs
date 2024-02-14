using Application.Trades.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace Application.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IMemoryCache _memoryCache;
        public string cacheKey = "trades";

        public TradeService(IHttpClientFactory factory, IMemoryCache memoryCache)
        {
            _factory = factory;
            _memoryCache = memoryCache;
        }       

        public async Task<TradeAverageModel> GetTradesAverage()
        {
            var trades = await GetTradesAsync();
            return CalculateTradesAverage(trades);
        }
        
        public async Task<TradeAverageModel> UpdateTradesAverage(CurrentTrade trade)
        {
            var recentTrade = GetRecentTrade(trade.Data);

            var trades = await GetTradesAsync();

            if (trades.Count == 0) throw new ArgumentNullException(nameof(trades));

            trades.Add(recentTrade);
            UpdateTradesInMemory(trades);           

            return CalculateTradesAverage(trades);

        }

        public TradeAverageModel CalculateTradesAverage(List<RecentTrade> trades)
        {
            var tradesCount = trades.Count;

            if (tradesCount == 0) throw new ArgumentNullException(nameof(trades));
            
            var tradesVolumeSum = trades.Sum(x => x.PrimaryCurrencyAmount);

            var minutes = GetMinutesDiff(trades.OrderBy(x => x.TradeTimestampUtc));

            var averageNumber = GetAverageNumber(tradesCount, minutes);

            var averageVolume = GetAverageVolume(tradesVolumeSum, minutes);

            return new TradeAverageModel() { AverageNumberPerMinute = averageNumber, AverageVolumePerMinute = averageVolume };
        }

        private async Task<List<RecentTrade>> GetTradesAsync()
        {
            List<RecentTrade?> trades;

            if (!_memoryCache.TryGetValue(cacheKey, out trades))
            {
                trades = await GetRecentTradesAsync();

                _memoryCache.Set(cacheKey, trades,
                    new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
            }

            return trades;
        }

        private void UpdateTradesInMemory(List<RecentTrade?> trades)
        {
            _memoryCache.Set(cacheKey, trades,
                    new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
        }

        private async Task<List<RecentTrade?>> GetRecentTradesAsync()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://api.independentreserve.com");

            var response = await client
                 .GetFromJsonAsync<RecentTradesResponse>("/Public/GetRecentTrades?primaryCurrencyCode=xbt&secondaryCurrencyCode=aud&numberOfRecentTradesToRetrieve=10");

            return response.Trades.ToList();
        }


        private RecentTrade GetRecentTrade(CurrentTradeDetails currentTrade)
        {
            return new RecentTrade()
            {
                TradeGuid = currentTrade.TradeGuid,
                PrimaryCurrencyAmount = currentTrade.Volume,
                TradeTimestampUtc = currentTrade.TradeDate.ToUniversalTime()
            };
        }

        private decimal GetAverageVolume(decimal tradesVolumeSum, double minutes)
        {
            return decimal.Round((tradesVolumeSum / (decimal)minutes),2, MidpointRounding.AwayFromZero);
        }

        private int GetAverageNumber(int tradesCount, double minutes)
        {
            return (int)(tradesCount / minutes);
        }

        private double GetMinutesDiff(IEnumerable<RecentTrade?> trades)
        {
            var firstTradeTime = trades.FirstOrDefault()?.TradeTimestampUtc;
            var lastTradeTime = trades.LastOrDefault()?.TradeTimestampUtc;

            var minutes = (lastTradeTime - firstTradeTime).Value.TotalMinutes;
            return minutes;
        }
    }
}
