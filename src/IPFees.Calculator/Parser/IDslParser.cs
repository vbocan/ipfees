namespace IPFees.Parser
{
    public interface IDslParser
    {
        IEnumerable<(DslError, string)> GetErrors();
        IEnumerable<DslFee> GetFees();
        IEnumerable<DslVariable> GetVariables();
        bool Parse(string source);
    }
}