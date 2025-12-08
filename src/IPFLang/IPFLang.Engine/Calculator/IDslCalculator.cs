using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFLang.Engine
{
    public interface IDslCalculator
    {
        void Reset();
        bool Parse(string text);
        (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) Compute(IEnumerable<IPFValue> InputValues);
        IEnumerable<string> GetErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslInput> GetInputs();
        IEnumerable<DslGroup> GetGroups();
        IEnumerable<DslReturn> GetReturns();
    }
}
