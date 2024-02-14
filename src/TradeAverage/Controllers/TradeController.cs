using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Trades.Models;

namespace TradeAverage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        public readonly ITradeService _tradeService;
        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet(Name = "GetTrageAverage")]
        public async Task<ActionResult<TradeAverageModel>> Get()
        {
            var result = await _tradeService.GetTradesAverage();
            return Ok(result);
        }

        [HttpPost(Name = "UpdateAverage")]
        public async Task<ActionResult<TradeAverageModel>> Post(CurrentTrade trade)
        {
            var result = await _tradeService.UpdateTradesAverage(trade);
            return Ok(result);
        }
    }
}

