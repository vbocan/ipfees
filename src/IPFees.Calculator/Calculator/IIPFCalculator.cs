using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Calculator
{
    public interface IIPFCalculator
    {
        bool Parse(string text);
        (double, double, IEnumerable<string>) Compute(IList<IPFValue> vars);
        IEnumerable<string> GetErrors();
        IEnumerable<IPFFee> GetFees();
        IEnumerable<IPFVariable> GetVariables();
    }
}