namespace Application.Trades.Models
{
    public class RecentTradesResponse
    {
        public IEnumerable<RecentTrade> Trades { get; set; }
    }

    public class RecentTrade
    {
        public Guid TradeGuid { get; set; }

        public DateTime TradeTimestampUtc { get; set; }

        public decimal PrimaryCurrencyAmount { get; set; }

        public decimal SecondaryCurrencyTradePrice { get; set; }

        public string? Taker { get; set; }
    }
}
