using System.Xml.Linq;

namespace IPFees.Parser
{
    public abstract record DslVariable(string Name, string Text);
    public record DslVariableBoolean(string Name, string Text, bool DefaultValue) : DslVariable(Name, Text);
    public record DslVariableList(string Name, string Text, IList<DslListItem> Items, string DefaultSymbol) : DslVariable(Name, Text);
    public record DslVariableListMultiple(string Name, string Text, IList<DslListItem> Items, IList<string> DefaultSymbols) : DslVariable(Name, Text);
    public record DslListItem(string Symbol, string Value);
    public record DslVariableNumber(string Name, string Text, int MinValue, int MaxValue, int DefaultValue) : DslVariable(Name, Text);
    public record DslVariableDate(string Name, string Text, DateOnly MinValue, DateOnly MaxValue, DateOnly DefaultValue) : DslVariable(Name, Text);

    public abstract record DslItem(IEnumerable<string> Condition);
    public record DslFee(string Name, bool Optional, IList<DslItem> Cases, IList<DslFeeVar> Vars)
    {
        public override string ToString()
        {
            if (Optional)
            {
                return string.Format("\n\rOPTIONAL FEE: {0}\n\r{2}\n\r{1}\n\r", Name, string.Join(Environment.NewLine, Cases), string.Join(Environment.NewLine, Vars));
            }
            else
            {
                return string.Format("\n\rFEE: {0}\n\r{2}\n\r{1}\n\r", Name, string.Join(Environment.NewLine, Cases), string.Join(Environment.NewLine, Vars));
            }
        }
    }
    public record DslFeeCase(IEnumerable<string> Condition, IList<DslFeeYield> Yields) : DslItem(Condition)
    {
        public override string ToString()
        {
            return string.Format("CASE: {0}\n\r{1}", string.Join(" ", Condition), string.Join(Environment.NewLine, Yields));
        }
    }
    public record DslFeeYield(IEnumerable<string> Condition, IEnumerable<string> Values) : DslItem(Condition)
    {
        public override string ToString()
        {
            return string.Format("YIELD: {0} CONDITION: [{1}]", string.Join(" ", Values), string.Join(" ", Condition));
        }
    }
    public record DslFeeVar(string Name, IEnumerable<string> ValueTokens)
    {
        public override string ToString()
        {
            return string.Format("VAR: {0} AS [{1}]", Name, string.Join(Environment.NewLine, ValueTokens));
        }
    }
    public record DslReturn(string Symbol, string Text)
    {
        public override string ToString()
        {
            return string.Format("RETURN: {0} AS [{1}]", Symbol, Text);
        }
    }
}
