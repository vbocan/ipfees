using Asp.Versioning;
using IPFees.API.Data;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFLang.Evaluator;
using IPFLang.Parser;
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

        /// <summary>
        /// Run the static verification directives declared by the fee schedules behind one or
        /// more jurisdictions, without computing anything. Answers whether a schedule covers
        /// every input combination and moves in the direction it promised.
        /// </summary>
        /// <param name="Jurisdictions">Comma-separated jurisdiction names.</param>
        [HttpGet("Verify/{Jurisdictions}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(IEnumerable<FeeVerificationInfo>), 200)]
        public IActionResult Verify(string Jurisdictions)
        {
            logger.LogInformation($"[REQUEST] Verify fee schedules for jurisdictions {Jurisdictions}.");

            var JurisdictionList = Jurisdictions.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (JurisdictionList.Length == 0)
            {
                return BadRequest("You need to supply one or more comma-separated jurisdictions.");
            }

            var results = jurisdictionFeeManager.Verify(JurisdictionList).ToList();
            if (results.Count == 0)
            {
                return NotFound($"No fee definitions are registered for {Jurisdictions}.");
            }

            return Ok(results);
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
            if (!TryCollectValues(Model.Parameters, out var CollectedValues, out var ParameterError))
            {
                return BadRequest(ParameterError);
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
            catch (Exception)
            {
                return BadRequest("Could not calculate fees based on the provided data.");
            }


        }

        /// <summary>
        /// Turn the wire representation of the calculation parameters into engine input values.
        /// A malformed value is reported rather than thrown, so a bad request reads as a 400
        /// naming the parameter instead of a 500.
        /// </summary>
        private bool TryCollectValues(IEnumerable<CalculationParameter> parameters, out List<IPFValue> values, out string error)
        {
            values = new List<IPFValue>();
            error = string.Empty;

            foreach (var par in parameters)
            {
                try
                {
                    if (par.Type.Equals(STR_BOOL, StringComparison.InvariantCultureIgnoreCase))
                    {
                        values.Add(new IPFValueBoolean(par.Name, bool.Parse(par.Value)));
                    }
                    else if (par.Type.Equals(STR_STRING, StringComparison.InvariantCultureIgnoreCase))
                    {
                        values.Add(new IPFValueString(par.Name, par.Value));
                    }
                    else if (par.Type.Equals(STR_MULTIPLESTRINGS, StringComparison.InvariantCultureIgnoreCase))
                    {
                        values.Add(new IPFValueStringList(par.Name, par.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));
                    }
                    else if (par.Type.Equals(STR_NUMBER, StringComparison.InvariantCultureIgnoreCase))
                    {
                        values.Add(new IPFValueNumber(par.Name, decimal.Parse(par.Value, CultureInfo.InvariantCulture)));
                    }
                    else if (par.Type.Equals(STR_DATE, StringComparison.InvariantCultureIgnoreCase))
                    {
                        values.Add(new IPFValueDate(par.Name, DateOnly.FromDateTime(DateTime.ParseExact(par.Value, STR_DATEFORMAT, CultureInfo.InvariantCulture))));
                    }
                    else
                    {
                        error = $"Unknown type '{par.Type}' for parameter '{par.Name}'. Must be one of Boolean, String, MultipleStrings, Number, Date.";
                        return false;
                    }
                }
                catch (Exception ex) when (ex is FormatException or ArgumentException)
                {
                    error = $"Value '{par.Value}' is not a valid {par.Type} for parameter '{par.Name}'.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compute the given jurisdictions and return why each amount arose rather than the
        /// amounts alone: which rules fired, which did not and on what condition, and what the
        /// inputs were worth at each step. Set <c>alternatives</c> to also report what the total
        /// would have been had a single input differed.
        /// </summary>
        [HttpPost("Explain"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(IEnumerable<FeeExplanation>), 200)]
        public IActionResult Explain([FromBody] CalculationViewModel Model, [FromQuery] bool alternatives = false)
        {
            logger.LogInformation($"[REQUEST] Explain calculation for jurisdictions {Model.Jurisdictions}.");

            var JurisdictionList = Model.Jurisdictions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (JurisdictionList.Length == 0)
            {
                return BadRequest("You need to supply one or more comma-separated jurisdictions.");
            }

            if (!TryCollectValues(Model.Parameters, out var CollectedValues, out var ParameterError))
            {
                return BadRequest(ParameterError);
            }

            var Explanations = jurisdictionFeeManager.Explain(JurisdictionList, CollectedValues, alternatives).ToList();
            if (Explanations.Count == 0)
            {
                return NotFound($"No fee definitions are registered for {Model.Jurisdictions}.");
            }

            return Ok(Explanations);
        }
    }
    public record CalculationParams(IEnumerable<object> Inputs);
    public record CalculationViewModel(string Jurisdictions, string TargetCurrency, IEnumerable<CalculationParameter> Parameters);
    public record CalculationParameter(string Type, string Name, string Value);
    public record CalculationResult(List<JurisdictionFeesAmount> JurisdictionFees, Fee TotalOfficialFee, Fee TotalPartnerFee, Fee TotalTranslationFee, Fee TotalServiceFee, Fee GrandTotalFee, IList<FeeResultFail> Errors);
}