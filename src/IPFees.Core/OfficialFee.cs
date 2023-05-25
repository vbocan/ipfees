using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using System.Linq;
using IPFees.Core.Repository;
using IPFees.Core.Model;

namespace IPFees.Core
{
    public class OfficialFee : IOfficialFee
    {
        private readonly IDslCalculator Calculator;
        private readonly IEnumerable<FeeInfo> Fees;
        private readonly IEnumerable<ModuleInfo> Modules;

        public OfficialFee(IFeeRepository fee, IModuleRepository module, IDslCalculator calculator)
        {
            Calculator = calculator;
            Fees = fee.GetFees().Result;
            Modules = module.GetModules().Result;
        }

        private FeeInfo? GetFeeById(Guid Id) => Fees.SingleOrDefault(w => w.Id.Equals(Id));
        private ModuleInfo? GetModuleById(Guid Id) => Modules.SingleOrDefault(w => w.Id.Equals(Id));

        /// <summary>
        /// Compute the official fees for the specified fee
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
            // Step 2: Parse the source code of the current fee
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
        /// Compute official fees for the all specified fees
        /// </summary>
        /// <param name="FeeIds">Fee Ids</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public IEnumerable<FeeResult> Calculate(IEnumerable<Guid> FeeIds, IList<IPFValue> InputValues)
        {
            foreach (var id in FeeIds) yield return Calculate(id, InputValues);
        }

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
            // Step 2: Parse the source code of the current fee
            var res = Calculator.Parse(jur.SourceCode);
            if (!res)
            {
                var Errors = Calculator.GetErrors();
                return new FeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var Inputs = Calculator.GetInputs();
                return new FeeResultParse(jur.Name, jur.Description, Inputs);
            }
        }

        public (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<Guid> FeeIds)
        {
            var Errors = new List<FeeResultFail>();
            var Inputs = new List<DslInput>();

            foreach (var id in FeeIds)
            {
                var inp = GetInputs(id);
                if (inp is FeeResultFail)
                {
                    Errors.Add(inp as FeeResultFail);
                }
                else
                {
                    var fps = inp as FeeResultParse;
                    Inputs.AddRange(fps.FeeInputs);
                }
            }

            var DedupedInputs = Inputs.DistinctBy(d => d.Name);
            return (DedupedInputs, Errors);
        }
    }

    public abstract record FeeResult();
    public record FeeResultFail(string FeeName, string FeeDescription, IEnumerable<string> Errors) : FeeResult();
    public record FeeResultCalculation(string FeeName, string FeeDescription, double TotalMandatoryAmount, double TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : FeeResult();
    public record FeeResultParse(string FeeName, string FeeDescription, IEnumerable<DslInput> FeeInputs) : FeeResult();
}
