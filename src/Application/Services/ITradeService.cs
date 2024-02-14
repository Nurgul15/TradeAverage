using Application.Trades.Models;
namespace Application.Services
{
    public interface ITradeService
    {
        Task<TradeAverageModel> GetTradesAverage();

        Task<TradeAverageModel> UpdateTradesAverage(CurrentTrade trade);

        TradeAverageModel CalculateTradesAverage(List<RecentTrade> trades);
    }
}
