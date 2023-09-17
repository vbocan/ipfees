using IPFees.Calculator;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Core.FeeCalculation
{
    public class FeeCalculator : IFeeCalculator
    {
        private readonly IDslCalculator Calculator;
        private readonly IEnumerable<FeeInfo> Fees;
        private readonly IEnumerable<ModuleInfo> Modules;

        public FeeCalculator(IFeeRepository fee, IModuleRepository module, IDslCalculator calculator)
        {
            Calculator = calculator;
            Fees = fee.GetFees().Result;
            Modules = module.GetModules().Result;
        }

        private FeeInfo? GetFeeById(Guid Id) => Fees.SingleOrDefault(w => w.Id.Equals(Id));
        private ModuleInfo? GetModuleById(Guid Id) => Modules.SingleOrDefault(w => w.Id.Equals(Id));

        /// <summary>
        /// Compute the specified fee
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public FeeResult Calculate(Guid FeeId, IList<IPFValue> InputValues)
        {
            // Reset calculator
            Calculator.Reset();
            var jur = GetFeeById(FeeId) ?? throw new NotSupportedException($"Fee '{FeeId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse autorun modules
            var AutoRunModules = Modules.Where(w => w.AutoRun);
            foreach (var arm in AutoRunModules)
            {
                // Retrieve the autorun module
                var mod = GetModuleById(arm.Id) ?? throw new NotSupportedException($"Module '{arm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 3: Parse the source code of the current fee
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var Errors = Calculator.GetErrors();
                return new FeeResultFail(jur.Name, jur.Description, Errors);
            }

            try
            {
                var (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns) = Calculator.Compute(InputValues);
                return new FeeResultCalculation(jur.Name, jur.Description, TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns);
            }
            catch (Exception ex)
            {
                return new FeeResultFail(jur.Name, jur.Description, new string[] { ex.Message });
            }
        }

        /// <summary>
        /// Get the inputs needed for the specified fee
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        /// <returns>List of inputs needed for invoking the fee calculation</returns>
        /// <exception cref="NotSupportedException"></exception>
        public FeeResult GetInputs(Guid FeeId)
        {
            // Reset calculator
            Calculator.Reset();
            var jur = GetFeeById(FeeId) ?? throw new NotSupportedException($"Fee '{FeeId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse autorun modules
            var AutoRunModules = Modules.Where(w => w.AutoRun);
            foreach (var arm in AutoRunModules)
            {
                // Retrieve the autorun module
                var mod = GetModuleById(arm.Id) ?? throw new NotSupportedException($"Module '{arm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 3: Parse the source code of the current fee
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var Errors = Calculator.GetErrors();
                return new FeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var Inputs = Calculator.GetInputs();
                var Groups = Calculator.GetGroups();
                return new FeeResultParse(jur.Name, jur.Description, Inputs, Groups);
            }
        }
    }

    public abstract record FeeResult();
    public record FeeResultFail(string FeeName, string FeeDescription, IEnumerable<string> Errors) : FeeResult();
    public record FeeResultCalculation(string FeeName, string FeeDescription, double TotalMandatoryAmount, double TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : FeeResult();
    public record FeeResultParse(string FeeName, string FeeDescription, IEnumerable<DslInput> FeeInputs, IEnumerable<DslGroup> FeeGroups) : FeeResult();
}
