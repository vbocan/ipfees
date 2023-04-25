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
            // Reset calculator
            Calculator.Reset();
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
                return new OfficialFeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns) = Calculator.Compute(Vars);
                return new OfficialFeeCalculationSuccess(jur.Name, jur.Description, TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns);
            }
        }

        /// <summary>
        /// Compute the official fees for the specified jurisdiction
        /// </summary>
        /// <param name="JurisdictionId">Jurisdiction Id</param>
        /// <param name="Vars">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public async IAsyncEnumerable<OfficialFeeResult> Calculate(IEnumerable<Guid> JurisdictionIds, IList<IPFValue> Vars)
        {
            foreach (var id in JurisdictionIds) yield return await Calculate(id, Vars);
        }

        public async Task<OfficialFeeResult> GetVariables(Guid JurisdictionId)
        {
            // Reset calculator
            Calculator.Reset();
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
                return new OfficialFeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var Vars = Calculator.GetVariables();
                return new OfficialFeeParseSuccess(jur.Name, jur.Description, Vars);
            }
        }

        public abstract record OfficialFeeResult(bool IsSuccessfull);
        public record OfficialFeeResultFail(string JurisdictionName, string JurisdictionDescription, IEnumerable<string> Errors) : OfficialFeeResult(false);
        public record OfficialFeeCalculationSuccess(string JurisdictionName, string JurisdictionDescription, double TotalMandatoryAmount, double TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : OfficialFeeResult(true);
        public record OfficialFeeParseSuccess(string JurisdictionName, string JurisdictionDescription, IEnumerable<DslVariable> ParsedVariables) : OfficialFeeResult(true);
    }
}
