using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Calculator
{
    public interface IDslCalculator
    {
        bool Parse(string text);
        (double, double, IEnumerable<string>, IEnumerable<(string, string)>) Compute(IList<IPFValue> vars);
        IEnumerable<string> GetErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslVariable> GetVariables();
        IEnumerable<DslReturn> GetReturns();
    }
}