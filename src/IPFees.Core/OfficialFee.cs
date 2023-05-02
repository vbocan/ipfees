using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using System.Linq;

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
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<FeeResult> Calculate(Guid JurisdictionId, IList<IPFValue> InputValues)
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
                return new FeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns) = Calculator.Compute(InputValues);
                return new FeeResultCalculation(jur.Name, jur.Description, TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns);
            }
        }

        /// <summary>
        /// Compute official fees for the all specified jurisdictions
        /// </summary>
        /// <param name="JurisdictionIds">Jurisdiction Ids</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public async IAsyncEnumerable<FeeResult> Calculate(IEnumerable<Guid> JurisdictionIds, IList<IPFValue> InputValues)
        {
            foreach (var id in JurisdictionIds) yield return await Calculate(id, InputValues);
        }

        public async Task<FeeResult> GetInputs(Guid JurisdictionId)
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
                return new FeeResultFail(jur.Name, jur.Description, Errors);
            }
            else
            {
                var Inputs = Calculator.GetInputs();
                return new FeeResultParse(jur.Name, jur.Description, Inputs);
            }
        }

        public async Task<(IEnumerable<DslInput>, IEnumerable<FeeResultFail>)> GetConsolidatedInputs(IEnumerable<Guid> JurisdictionIds)
        {
            var Errors = new List<FeeResultFail>();
            var Inputs = new List<DslInput>();

            foreach (var id in JurisdictionIds)
            {
                var StartTime = DateTime.Now;
                Console.Write("Getting input...");
                var inp = await GetInputs(id);
                var EndTime = DateTime.Now;
                Console.WriteLine($"in {(EndTime-StartTime).TotalMilliseconds} ms.");
                if (inp is FeeResultFail)
                {
                    Errors.Add(inp as FeeResultFail);
                }
                else
                {
                    var fps = inp as FeeResultParse;
                    Inputs.AddRange(fps.JurisdictionInputs);
                }
            }

            var DedupedInputs = Inputs.DistinctBy(d => d.Name);
            return (DedupedInputs, Errors);
        }
    }

    public abstract record FeeResult();
    public record FeeResultFail(string JurisdictionName, string JurisdictionDescription, IEnumerable<string> Errors) : FeeResult();
    public record FeeResultCalculation(string JurisdictionName, string JurisdictionDescription, double TotalMandatoryAmount, double TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : FeeResult();
    public record FeeResultParse(string JurisdictionName, string JurisdictionDescription, IEnumerable<DslInput> JurisdictionInputs) : FeeResult();
}
