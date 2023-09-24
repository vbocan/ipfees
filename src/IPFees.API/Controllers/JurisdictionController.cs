using IPFees.Core.CurrencyConversion;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class JurisdictionController : ControllerBase
    {
        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly ILogger<JurisdictionController> logger;

        public JurisdictionController(IJurisdictionRepository jurisdictionRepository, ILogger<JurisdictionController> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.logger = logger;            
        }

        [HttpGet(Name = "GetJurisdictions"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(IEnumerable<JurisdictionItem>), 200)]
        public async Task<IActionResult> GetJurisdictions()
        {
            logger.LogInformation("Currency information requested by the client");
            var Jurisdictions = await jurisdictionRepository.GetJurisdictions();
            var JurisdictionList = Jurisdictions.Select(s => new JurisdictionItem(s.Name, s.Description));
            logger.LogInformation($"Information about {Jurisdictions.Count()} jurisdictions have been delivered.");
            return Ok(JurisdictionList);
        }        
    }
    public record JurisdictionItem(string Jurisdiction, string Description);
}