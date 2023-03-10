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
        /// <param name="JurisdictionName">JurisdictionRepository name</param>
        /// <param name="Vars">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<OfficialFeeResult> Calculate(string JurisdictionName, IList<IPFValue> Vars)
        {
            var jur = await Jurisdiction.GetJurisdictionByName(JurisdictionName) ?? throw new NotSupportedException($"JurisdictionRepository '{JurisdictionName}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = await Module.GetModuleByName(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
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
                var (TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps) = Calculator.Compute(Vars);
                return new OfficialFeeCalculationSuccess(TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps);
            }
        }

        public async Task<OfficialFeeResult> GetVariables(string JurisdictionName)
        {
            var jur = await Jurisdiction.GetJurisdictionByName(JurisdictionName) ?? throw new NotSupportedException($"JurisdictionRepository '{JurisdictionName}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = await Module.GetModuleByName(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
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
        public record OfficialFeeCalculationSuccess(double TotalMandatoryAmount, double TotalOptionalAMount, IEnumerable<string> CalculationSteps) : OfficialFeeResult(true);
        public record OfficialFeeParseSuccess(IEnumerable<DslVariable> RequestedVariables) : OfficialFeeResult(true);
    }
}
