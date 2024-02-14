namespace Application.Trades.Models
{
    public class CurrentTrade
    {
        public string Channel { get; set; }
        public int Nonce { get; set; }
        public CurrentTradeDetails Data { get; set; }
        public long Time { get; set; }
        public string Event { get; set; }
    }
}
