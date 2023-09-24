using IPFees.Core.CurrencyConversion;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Parser;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class ComputeController : ControllerBase
    {
        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<ComputeController> logger;

        public ComputeController(IJurisdictionFeeManager jurisdictionFeeManager, ILogger<ComputeController> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.logger = logger;
        }


        [HttpGet("GetParameters/{Jurisdictions}/{TargetCurrency}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(ComputeParams), 200)]
        public IActionResult GetParameters(string Jurisdictions, string TargetCurrency)
        {
            logger.LogInformation($"[REQUEST] Get parameters for jurisdictions {Jurisdictions} and target currency {TargetCurrency}");
            var JurArray = Jurisdictions.Split(",");
            var (inputs, groups, errs) = jurisdictionFeeManager.GetConsolidatedInputs(JurArray);
            var response = new ComputeParams(inputs, groups, errs);
            return Ok(response);
        }
    }

    public record ComputeParams(IEnumerable<DslInput> ExpectedParameters, IEnumerable<DslGroup> Groups, IEnumerable<FeeResultFail> Errors);
}