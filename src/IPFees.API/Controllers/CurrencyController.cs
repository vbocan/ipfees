using IPFees.Core.CurrencyConversion;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]    
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConverter serd;
        private readonly ILogger<CurrencyController> logger;

        public CurrencyController(ICurrencyConverter serd, ILogger<CurrencyController> logger)
        {
            this.serd = serd;
            this.logger = logger;
        }

        [HttpGet(Name = "GetCurrencies"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyItem>), 200)]
        public IActionResult GetCurrencies()
        {
            logger.LogInformation("Currency information requested by the client");
            var Currencies = serd.GetCurrencies().Select(s=>new CurrencyItem(s.Item1, s.Item2));
            logger.LogInformation($"Information about {Currencies.Count()} currencies have been delivered.");
            return Ok(Currencies);
        }        
    }
    public record CurrencyItem(string Currency, string Description);
}