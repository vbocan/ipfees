using IPFLang.CurrencyConversion;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFLang.Types;

namespace IPFLang.Engine
{
    public interface IDslCalculator
    {
        void Reset();
        bool Parse(string text);
        (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) Compute(IEnumerable<IPFValue> InputValues);
        IEnumerable<string> GetErrors();
        IEnumerable<TypeError> GetTypeErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslInput> GetInputs();
        IEnumerable<DslGroup> GetGroups();
        IEnumerable<DslReturn> GetReturns();
        void SetCurrencyConverter(ICurrencyConverter converter);
    }
}
