namespace IPFees.Parser
{
    public abstract record IPFVariable(string Name, string Text);
    public record IPFVariableBoolean(string Name, string Text, bool DefaultValue) : IPFVariable(Name, Text);
    public record IPFVariableList(string Name, string Text, IList<IPFListItem> Items, string DefaultSymbol) : IPFVariable(Name, Text);
    public record IPFListItem(string Symbol, string Value);
    public record IPFVariableNumber(string Name, string Text, int MinValue, int MaxValue, int DefaultValue) : IPFVariable(Name, Text);

    public abstract record IPFItem(IEnumerable<string> Condition);
    public record IPFFee(string Name, bool Optional, IList<IPFItem> Cases, IList<IPFFeeVar> Vars)
    {
        public override string ToString()
        {
            if (Optional)
            {
                return string.Format("\n\rOPTIONAL FEE: {0}\n\r{1}", Name, string.Join(Environment.NewLine, Cases));
            }
            else
            {
                return string.Format("\n\rFEE: {0}\n\r{1}", Name, string.Join(Environment.NewLine, Cases));
            }
        }
    }
    public record IPFFeeCase(IEnumerable<string> Condition, IList<IPFFeeYield> Yields) : IPFItem(Condition)
    {
        public override string ToString()
        {
            return string.Format("CASE: {0}\n\r{1}", string.Join(" ", Condition), string.Join(Environment.NewLine, Yields));
        }
    }
    public record IPFFeeYield(IEnumerable<string> Condition, IEnumerable<string> Values) : IPFItem(Condition)
    {
        public override string ToString()
        {
            return string.Format("YIELD: {0} CONDITION: {1}", string.Join(" ", Values), string.Join(" ", Condition));
        }
    }
    public record IPFFeeVar(string Name, IEnumerable<string> ValueTokens)
    {
        public override string ToString()
        {
            return string.Format("VAR: {0}\n\r{1}", Name, string.Join(Environment.NewLine, ValueTokens));
        }
    }
}
