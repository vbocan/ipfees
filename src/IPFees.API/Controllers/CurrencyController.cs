using IPFees.Core.CurrencyConversion;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConverter serd;
        private readonly ILogger<CurrencyController> logger;

        public CurrencyController(ICurrencyConverter serd, ILogger<CurrencyController> logger)
        {
            this.serd = serd;
            this.logger = logger;
        }

        [HttpGet(Name = "GetCurrencies")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyItem>), 200)]
        public IActionResult Get()
        {
            logger.LogInformation("Currency information requested by the client");
            var Currencies = serd.GetCurrencies().Select(s=>new CurrencyItem(s.Item1, s.Item2));
            logger.LogInformation($"Information about {Currencies.Count()} currencies have been delivered.");
            return Ok(Currencies);
        }        
    }
    public record CurrencyItem(string Currency, string Description);
}