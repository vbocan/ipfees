using Asp.Versioning;
using IPFees.Core.FeeManager;
using IPFees.Parser;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("CalculateFees/{Jurisdictions}/{TargetCurrency}"), MapToApiVersion("1")]
        //[ProducesResponseType(typeof(CalculationParams), 200)]
        public IActionResult CalculateFees(string Jurisdictions, string TargetCurrency)
        {
            //var CollectedValues = new List<IPFValue>();

            //// Cycle through all form fields to build the collected values list
            //foreach (var item in Inputs)
            //{
            //    if (item.Type == typeof(DslInputList).ToString())
            //    {
            //        // A single-selection list return a string
            //        CollectedValues.Add(new IPFValueString(item.Name, item.StrValue));
            //    }
            //    else if (item.Type == typeof(DslInputListMultiple).ToString())
            //    {
            //        // A multiple-selection list return a string list
            //        CollectedValues.Add(new IPFValueStringList(item.Name, item.ListValue));
            //    }
            //    else if (item.Type == typeof(DslInputNumber).ToString())
            //    {
            //        // A number input returns a double
            //        CollectedValues.Add(new IPFValueNumber(item.Name, item.DecimalValue));
            //    }
            //    else if (item.Type == typeof(DslInputBoolean).ToString())
            //    {
            //        // A boolean input returns a boolean
            //        CollectedValues.Add(new IPFValueBoolean(item.Name, item.BoolValue));
            //    }
            //    else if (item.Type == typeof(DslInputDate).ToString())
            //    {
            //        // A date input returns a date
            //        CollectedValues.Add(new IPFValueDate(item.Name, item.DateValue));
            //    }
            //}

            //FeeResults = await jurisdictionFeeManager.Calculate(SelectedJurisdictions.AsEnumerable(), CollectedValues, TargetCurrency, currencySettings.CurrencyMarkup);
            return Ok();
        }
    }
    public record CalculationParams(IEnumerable<object> Inputs);
}