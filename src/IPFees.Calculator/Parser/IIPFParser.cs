namespace IPFees.Parser
{
    public interface IIPFParser
    {
        IEnumerable<(IPFError, string)> GetErrors();
        IEnumerable<IPFFee> GetFees();
        IEnumerable<IPFVariable> GetVariables();
        bool Parse(string source);
    }
}