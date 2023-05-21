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
        private readonly IEnumerable<JurisdictionInfo> Jurisdictions;
        private readonly IEnumerable<ModuleInfo> Modules;

        public OfficialFee(IJurisdictionRepository jurisdiction, IModuleRepository module, IDslCalculator calculator)
        {
            Calculator = calculator;
            Jurisdictions = jurisdiction.GetJurisdictions().Result;
            Modules = module.GetModules().Result;
        }

        private JurisdictionInfo? GetJurisdictionById(Guid Id) => Jurisdictions.SingleOrDefault(w => w.Id.Equals(Id));
        private ModuleInfo? GetModuleById(Guid Id) => Modules.SingleOrDefault(w => w.Id.Equals(Id));

        /// <summary>
        /// Compute the official fees for the specified jurisdiction
        /// </summary>
        /// <param name="JurisdictionId">Jurisdiction Id</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public FeeResult Calculate(Guid JurisdictionId, IList<IPFValue> InputValues)
        {
            // Reset calculator
            Calculator.Reset();
            var jur = GetJurisdictionById(JurisdictionId) ?? throw new NotSupportedException($"Jurisdiction '{JurisdictionId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                Calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse the source code of the current jurisdiction
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
        /// Compute official fees for the all specified jurisdictions
        /// </summary>
        /// <param name="JurisdictionIds">Jurisdiction Ids</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <exception cref="NotSupportedException"></exception>
        public IEnumerable<FeeResult> Calculate(IEnumerable<Guid> JurisdictionIds, IList<IPFValue> InputValues)
        {
            foreach (var id in JurisdictionIds) yield return Calculate(id, InputValues);
        }

        public FeeResult GetInputs(Guid JurisdictionId)
        {
            // Reset calculator
            Calculator.Reset();
            var jur = GetJurisdictionById(JurisdictionId) ?? throw new NotSupportedException($"Jurisdiction '{JurisdictionId}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = GetModuleById(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
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

        public (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<Guid> JurisdictionIds)
        {
            var Errors = new List<FeeResultFail>();
            var Inputs = new List<DslInput>();

            foreach (var id in JurisdictionIds)
            {
                var inp = GetInputs(id);
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
