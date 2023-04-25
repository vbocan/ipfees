using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;

namespace IPFees.Core
{
    public class OfficialFee : IOfficialFee
    {
        private IJurisdictionRepository Jurisdiction { get; set; }
        private IModuleRepository Module { get; set; }
        private IDslCalculator Calculator { get; set; }

        public OfficialFee(IJurisdictionRepository jurisdiction, IModuleRepository module, IDslCalculator calculator)
        {
            Jurisdiction = jurisdiction;
            Module = module;
            Calculator = calculator;
        }

        /// <summary>
        /// Compute the official fees for the specified jurisdiction
        /// </summary>
        /// <param name="JurisdictionId">Jurisdiction Id</param>
        /// <param name="Vars">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<OfficialFeeResult> Calculate(Guid JurisdictionId, IList<IPFValue> Vars)
        {
            var jur = await Jurisdiction.GetJurisdictionById(JurisdictionId) ?? throw new NotSupportedException($"Jurisdiction '{JurisdictionId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = await Module.GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse the source code of the current jurisdiction
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var Errors = Calculator.GetErrors();
                return new OfficialFeeResultFail(Errors);
            }
            else
            {
                var (TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps, Returns) = Calculator.Compute(Vars);
                return new OfficialFeeCalculationSuccess(TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps, Returns);
            }
        }

        public async Task<OfficialFeeResult> GetVariables(Guid JurisdictionId)
        {
            var jur = await Jurisdiction.GetJurisdictionById(JurisdictionId) ?? throw new NotSupportedException($"Jurisdiction '{JurisdictionId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = await Module.GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse the source code of the current jurisdiction
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var Errors = Calculator.GetErrors();
                return new OfficialFeeResultFail(Errors);
            }
            else
            {
                var Vars = Calculator.GetVariables();
                return new OfficialFeeParseSuccess(Vars);
            }
        }

        public abstract record OfficialFeeResult(bool IsSuccessfull);
        public record OfficialFeeResultFail(IEnumerable<string> Errors) : OfficialFeeResult(false);
        public record OfficialFeeCalculationSuccess(double TotalMandatoryAmount, double TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : OfficialFeeResult(true);
        public record OfficialFeeParseSuccess(IEnumerable<DslVariable> ParsedVariables) : OfficialFeeResult(true);
    }
}
