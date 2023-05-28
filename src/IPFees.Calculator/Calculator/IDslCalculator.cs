using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Calculator
{
    public interface IDslCalculator
    {
        void Reset();
        bool Parse(string text);
        (double, double, IEnumerable<string>, IEnumerable<(string, string)>) Compute(IEnumerable<IPFValue> InputValues);
        IEnumerable<string> GetErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslInput> GetInputs();
        IEnumerable<DslGroup> GetGroups();
        IEnumerable<DslReturn> GetReturns();
    }
}