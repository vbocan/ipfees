using System.Runtime.CompilerServices;

namespace IPFees.Parser
{
    public interface IDslParser
    {
        IEnumerable<(DslError, string)> GetErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslVariable> GetVariables();
        IEnumerable<DslReturn> GetReturns();
        bool Parse(string source);
        void Reset();
    }
}