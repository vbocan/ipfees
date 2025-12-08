namespace IPFLang.Evaluator
{
    public interface IContext
    {
        decimal ResolveVariable(string name);
        decimal CallFunction(string name, decimal[] arguments);
    }
}
