using IPFees.Calculator;
using IPFees.Evaluator;
using IPFFees.Core;

namespace IPFees.Core
{
    public class OfficialFee : IOfficialFee
    {
        private IJurisdictionRepository Jurisdiction { get; set; }
        private IModuleRepository Module { get; set; }
        private IIPFCalculator Calculator { get; set; }

        public OfficialFee(IJurisdictionRepository jurisdiction, IModuleRepository module, IIPFCalculator calculator)
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
        public CalculationResultBase Calculate(string JurisdictionName, IList<IPFValue> Vars)
        {
            var jur = Jurisdiction.GetJurisdictionByName(JurisdictionName) ?? throw new NotSupportedException($"JurisdictionRepository '{JurisdictionName}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = Module.GetModuleByName(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse the source code of the current jurisdiction
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var CalcErrors = Calculator.GetErrors();
                return new CalculationResultFail(CalcErrors);
            }
            else
            {
                var (TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps) = Calculator.Compute(Vars);
                return new CalculationResultSuccess(TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps);
            }
        }

        public abstract record CalculationResultBase(bool IsSuccessfull);
        public record CalculationResultFail(IEnumerable<string> CalculationErrors) : CalculationResultBase(false);
        public record CalculationResultSuccess(double TotalMandatoryAmount, double TotalOptionalAMount, IEnumerable<string> CalculationSteps) : CalculationResultBase(true);
    }
}
