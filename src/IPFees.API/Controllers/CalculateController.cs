using Asp.Versioning;
using IPFees.Core.FeeManager;
using IPFees.Evaluator;
using IPFees.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace IPFees.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class CalculateController : ControllerBase
    {
        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<CalculateController> logger;

        public CalculateController(IJurisdictionFeeManager jurisdictionFeeManager, ILogger<CalculateController> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.logger = logger;
        }


        [HttpGet("GetCalculationParameters/{Jurisdictions}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(CalculationParams), 200)]
        public IActionResult GetCalculationParameters(string Jurisdictions)
        {
            logger.LogInformation($"[REQUEST] Get calculation parameters for jurisdictions {Jurisdictions}.");
            var JurArray = Jurisdictions.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var (Inputs, Groups, Errors) = jurisdictionFeeManager.GetConsolidatedInputs(JurArray);

            if (Errors.Any()) return StatusCode(500);

            var response = new List<object>();

            foreach (var inp in Inputs)
            {
                if (inp is DslInputBoolean obj1)
                {
                    var serobj = new
                    {
                        Type = "Boolean",
                        Name = obj1.Name,
                        ExpectedValues = new bool[] { true, false }
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputList obj2)
                {
                    var serobj = new
                    {
                        Type = "String",
                        Name = obj2.Name,
                        ExpectedValues = obj2.Items.Select(s => s.Symbol)
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputListMultiple obj3)
                {
                    var serobj = new
                    {
                        Type = "MultipleStrings",
                        Name = obj3.Name,
                        ExpectedValues = obj3.Items.Select(s => s.Value)
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputNumber obj4)
                {
                    var serobj = new
                    {
                        Type = "Number",
                        Name = obj4.Name,
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputDate obj5)
                {
                    var serobj = new
                    {
                        Type = "Date",
                        Name = obj5.Name,
                        Format = "yyyy-mm-dd",
                    };
                    response.Add(JsonSerializer.Serialize(serobj));
                }
            }
            return Ok(new CalculationParams(response));
        }

        [HttpGet("CalculateFees/{Jurisdictions}/{TargetCurrency}/{CurrencyMarkup}/{Parameters}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(TotalFeeInfo), 200)]
        public async Task<IActionResult> CalculateFees(string Jurisdictions, string TargetCurrency, decimal CurrencyMarkup, string Parameters)
        {
            // Split jurisdictions at comma
            var JurisdictionList = Jurisdictions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            // Decode calculation parameters
            var ParameterList = JsonSerializer.Deserialize<IEnumerable<CalculationValue>>(Parameters);

            var CollectedValues = new List<IPFValue>();
            foreach (var par in ParameterList)
            {
                // Input parameter is a boolean
                if (par.Type.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = bool.Parse(par.Value);
                    CollectedValues.Add(new IPFValueBoolean(Name, Value));
                }
                // Input parameter is a string
                else if (par.Type.Equals("String", StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = par.Value;
                    CollectedValues.Add(new IPFValueString(Name, Value));
                }
                // Input parameter is a list of comma-separated strings
                else if (par.Type.Equals("MultipleStrings", StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = par.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    CollectedValues.Add(new IPFValueStringList(Name, Value));
                }
                // Input parameter is a number
                else if (par.Type.Equals("Number", StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = decimal.Parse(par.Value);
                    CollectedValues.Add(new IPFValueNumber(Name, Value));
                }
                // Input parameter is a date in the format yyyy-mm-dd
                else if (par.Type.Equals("Date", StringComparison.InvariantCultureIgnoreCase))
                {
                    var Name = par.Name;
                    var Value = DateOnly.FromDateTime(DateTime.ParseExact(par.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    CollectedValues.Add(new IPFValueDate(Name, Value));
                }
            }            

            var FeeResults = await jurisdictionFeeManager.Calculate(JurisdictionList, CollectedValues, TargetCurrency, CurrencyMarkup);
            return Ok(FeeResults);
        }
    }
    public record CalculationParams(IEnumerable<object> Inputs);

    public record CalculationValue(string Type, string Name, string Value);
}