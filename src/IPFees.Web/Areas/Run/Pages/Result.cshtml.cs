using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ResultModel : PageModel
    {
        [BindProperty] public string ComputationError { get; set; } = null!;
        [BindProperty] public List<IPFValue> CollectedValues { get; set; } = null!;
        [BindProperty] public IList<InputViewModel> Inputs { get; set; } = null!;
        [BindProperty] public string[] SelectedJurisdictions { get; set; } = null!;
        [BindProperty] public string TargetCurrency { get; set; } = null!;
        [BindProperty] public TotalFeeInfo FeeResults { get; set; } = null!;

        /// <summary>
        /// Why each amount arose, as recorded by the engine while it computed. Shown beneath the
        /// figures so that a practitioner can see which rule produced a charge, and equally which
        /// rule did not fire and on what condition.
        /// </summary>
        public IReadOnlyList<FeeExplanation> Explanations { get; private set; } = Array.Empty<FeeExplanation>();
        private readonly CurrencySettings currencySettings;
        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<ResultModel> _logger;

        public ResultModel(IJurisdictionFeeManager jurisdictionFeeManager, IOptions<CurrencySettings> currencySettings, ILogger<ResultModel> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.currencySettings = currencySettings.Value;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields to build the collected values list
            foreach (var item in Inputs)
            {
                if (item.Type == typeof(DslInputList).ToString())
                {
                    // A single-selection list return a string
                    CollectedValues.Add(new IPFValueString(item.Name, item.StrValue));
                }
                else if (item.Type == typeof(DslInputListMultiple).ToString())
                {
                    // A multiple-selection list return a string list
                    CollectedValues.Add(new IPFValueStringList(item.Name, item.ListValue));
                }
                else if (item.Type == typeof(DslInputNumber).ToString())
                {
                    // A number input returns a double
                    CollectedValues.Add(new IPFValueNumber(item.Name, item.DecimalValue));
                }
                else if (item.Type == typeof(DslInputBoolean).ToString())
                {
                    // A boolean input returns a boolean
                    CollectedValues.Add(new IPFValueBoolean(item.Name, item.BoolValue));
                }
                else if (item.Type == typeof(DslInputDate).ToString())
                {
                    // A date input returns a date
                    CollectedValues.Add(new IPFValueDate(item.Name, item.DateValue));
                }
            }

            FeeResults = await jurisdictionFeeManager.Calculate(SelectedJurisdictions.AsEnumerable(), CollectedValues, TargetCurrency, currencySettings.CurrencyMarkup);

            // The audit trail is gathered alongside the totals rather than behind a second
            // request, so that what is explained is the calculation the reader is looking at.
            try
            {
                Explanations = jurisdictionFeeManager.Explain(SelectedJurisdictions.AsEnumerable(), CollectedValues).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not build the calculation audit trail: {Reason}", ex.Message);
            }

            return Page();
        }
    }
}
