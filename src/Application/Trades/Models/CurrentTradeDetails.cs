namespace Application.Trades.Models
{
    public class CurrentTradeDetails
    {
        public Guid TradeGuid { get; set; }
        public DateTime TradeDate { get; set; }
        public decimal Volume { get; set; }
        public Guid BidGuid { get; set; }
        public Guid OfferGuid { get; set; }
        public string? Side { get; set; }
        public Price Price { get; set; }

    }

    public class Price
    {
        public decimal Aud { get; set; }
    }
}
