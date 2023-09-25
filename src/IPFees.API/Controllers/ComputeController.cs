using IPFees.Core.CurrencyConversion;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Evaluator;
using IPFees.Parser;
using Mapster.Adapters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json;

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


        [HttpGet("GetFeeParameters/{Jurisdictions}"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(ComputeParams), 200)]
        public IActionResult GetFeeParameters(string Jurisdictions)
        {
            logger.LogInformation($"[REQUEST] Get input parameters for jurisdictions {Jurisdictions}.");
            var JurArray = Jurisdictions.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var (Inputs, Groups, Errors) = jurisdictionFeeManager.GetConsolidatedInputs(JurArray);

            if (Errors.Any()) return StatusCode(500);

            var ExpandedInputs = new List<object>();

            foreach (var inp in Inputs)
            {
                if (inp is DslInputBoolean obj1)
                {
                    var serobj = new
                    {
                        Display = new
                        {
                            Type = "Boolean",
                            Name = obj1.Name,
                            Text = obj1.Text,
                            Group = obj1.Group,
                            DefaultValue = obj1.DefaultValue
                        },
                        ExpectedInput = new
                        {
                            Type = "Boolean",
                        }
                    };
                    ExpandedInputs.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputList obj2)
                {
                    var serobj = new
                    {
                        Type = "List",
                        Name = obj2.Name,
                        Text = obj2.Text,
                        Group = obj2.Group,
                        DefaultSymbol = obj2.DefaultSymbol,
                        Items = obj2.Items.Select(s => new { Symbol = s.Symbol, Value = s.Value })
                    };
                    ExpandedInputs.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputListMultiple obj3)
                {
                    var serobj = new
                    {
                        Type = "ListMultiple",
                        Name = obj3.Name,
                        Text = obj3.Text,
                        Group = obj3.Group,
                        DefaultSymbols = obj3.DefaultSymbols.ToArray(),
                        Items = obj3.Items.Select(s => new { Symbol = s.Symbol, Value = s.Value })
                    };
                    ExpandedInputs.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputNumber obj4)
                {
                    var serobj = new
                    {
                        Type = "Number",
                        Name = obj4.Name,
                        Text = obj4.Text,
                        Group = obj4.Group,
                        MinValue = obj4.MinValue,
                        MaxValue = obj4.MaxValue,
                        DefaultValue = obj4.DefaultValue
                    };
                    ExpandedInputs.Add(JsonSerializer.Serialize(serobj));
                }
                else if (inp is DslInputDate obj5)
                {
                    var serobj = new
                    {
                        Type = "Date",
                        Format = "yyyy-mm-dd",
                        Name = obj5.Name,
                        Text = obj5.Text,
                        Group = obj5.Group,
                        MinValue = obj5.MinValue.ToString("yyyy-MM-dd"),
                        MaxValue = obj5.MaxValue.ToString("yyyy-MM-dd"),
                        DefaultValue = obj5.DefaultValue.ToString("yyyy-MM-dd")
                    };
                    ExpandedInputs.Add(JsonSerializer.Serialize(serobj));
                }
            }

            var response = new ComputeParams(ExpandedInputs, Groups);
            return Ok(response);
        }

        [HttpGet("ComputeFees/{Jurisdictions}/{TargetCurrency}"), MapToApiVersion("1")]
        //[ProducesResponseType(typeof(ComputeParams), 200)]
        public IActionResult ComputeFees(string Jurisdictions, string TargetCurrency)
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

    public record ComputeParams(IEnumerable<object> ExpectedInputs, IEnumerable<DslGroup> Groups);
}