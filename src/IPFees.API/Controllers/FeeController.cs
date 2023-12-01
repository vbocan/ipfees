using Asp.Versioning;
using IPFees.API.Attributes;
using IPFees.API.Data;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Evaluator;
using IPFees.Parser;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiKey]
    public class FeeController : ControllerBase
    {
        #region Constants
        private const string STR_BOOL = "Boolean";
        private const string STR_STRING = "String";
        private const string STR_MULTIPLESTRINGS = "MultipleStrings";
        private const string STR_NUMBER = "Number";
        private const string STR_DATE = "Date";
        private const string STR_DATEFORMAT = "yyyy-MM-dd";
        #endregion

        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<FeeController> logger;
        private readonly CurrencySettings currencySettings;

        public FeeController(IJurisdictionFeeManager jurisdictionFeeManager, IOptions<CurrencySettings> currencySettings, ILogger<FeeController> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.currencySettings = currencySettings.Value;
            this.logger = logger;
        }


        [HttpGet("Parameters/{Jurisdictions}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(CalculationParams), 200)]
        public IActionResult Parameters(string Jurisdictions)
        {
            logger.LogInformation($"[REQUEST] Get calculation parameters for jurisdictions {Jurisdictions}.");
            // Split the parameter at commas
            var JurisdictionList = Jurisdictions.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (JurisdictionList.Length == 0)
            {
                return BadRequest("You need to supply one or more comma-separated jurisdictions.");
            }

            var (Inputs, Groups, Errors) = jurisdictionFeeManager.GetConsolidatedInputs(JurisdictionList);
            // If there are any errors, there must be a serious internal server error
            if (Errors.Any())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var response = new List<object>();
            foreach (var inp in Inputs)
            {
                if (inp is DslInputBoolean obj1)
                {
                    var serobj = new
                    {
                        Type = STR_BOOL,
                        Name = obj1.Name,
                        ExpectedValues = new bool[] { true, false }
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputList obj2)
                {
                    var serobj = new
                    {
                        Type = STR_STRING,
                        Name = obj2.Name,
                        ExpectedValues = obj2.Items.Select(s => s.Symbol)
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputListMultiple obj3)
                {
                    var serobj = new
                    {
                        Type = STR_MULTIPLESTRINGS,
                        Name = obj3.Name,
                        ExpectedValues = obj3.Items.Select(s => s.Value)
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputNumber obj4)
                {
                    var serobj = new
                    {
                        Type = STR_NUMBER,
                        Name = obj4.Name,
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputDate obj5)
                {
                    var serobj = new
                    {
                        Type = STR_DATE,
                        Name = obj5.Name,
                        Format = STR_DATEFORMAT,
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
            }
            return Ok(new CalculationParams(response));
        }

        [HttpPost("Calculate"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(TotalFeeInfo), 200)]
        public async Task<IActionResult> Calculate([FromBody] CalculationViewModel Model)
        {
            logger.LogInformation($"[REQUEST] Perform calculation for jurisdictions {Model.Jurisdictions}.");

            // Split jurisdictions at comma
            var JurisdictionList = Model.Jurisdictions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (JurisdictionList.Length == 0)
            {
                return BadRequest("You need to supply one or more comma-separated jurisdictions.");
            }
            var CollectedValues = new List<IPFValue>();
            foreach (var par in Model.Parameters)
            {
                // Input parameter is a boolean
                if (par.Type.Equals(STR_BOOL, StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = bool.Parse(par.Value);
                    CollectedValues.Add(new IPFValueBoolean(Name, Value));
                }
                // Input parameter is a string
                else if (par.Type.Equals(STR_STRING, StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = par.Value;
                    CollectedValues.Add(new IPFValueString(Name, Value));
                }
                // Input parameter is a list of comma-separated strings
                else if (par.Type.Equals(STR_MULTIPLESTRINGS, StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = par.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    CollectedValues.Add(new IPFValueStringList(Name, Value));
                }
                // Input parameter is a number
                else if (par.Type.Equals(STR_NUMBER, StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = decimal.Parse(par.Value);
                    CollectedValues.Add(new IPFValueNumber(Name, Value));
                }
                // Input parameter is a date in the format yyyy-mm-dd
                else if (par.Type.Equals(STR_DATE, StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = DateOnly.FromDateTime(DateTime.ParseExact(par.Value, STR_DATEFORMAT, CultureInfo.InvariantCulture));
                    CollectedValues.Add(new IPFValueDate(Name, Value));
                }
                else
                {
                    return BadRequest("Unknown input type. Must be either Bool, String, MultipleStrings, Number, Date.");
                }
            }
            try
            {
                var FeeResults = await jurisdictionFeeManager.Calculate(JurisdictionList, CollectedValues, Model.TargetCurrency, currencySettings.CurrencyMarkup);
                if (FeeResults.Errors.Any())
                {
                    // Errors have occured while calculating
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                // We're not returning a POCO, so we need a dedicated object
                var CalculationResult = FeeResults.Adapt<CalculationResult>();
                return Ok(CalculationResult);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not calculate fees based on the provided data.");
            }


        }
    }
    public record CalculationParams(IEnumerable<object> Inputs);
    public record CalculationViewModel(string Jurisdictions, string TargetCurrency, IEnumerable<CalculationParameter> Parameters);
    public record CalculationParameter(string Type, string Name, string Value);
    public record CalculationResult(List<JurisdictionFeesAmount> JurisdictionFees, Fee TotalOfficialFee, Fee TotalPartnerFee, Fee TotalTranslationFee, Fee TotalServiceFee, Fee GrandTotalFee, IList<FeeResultFail> Errors);
}