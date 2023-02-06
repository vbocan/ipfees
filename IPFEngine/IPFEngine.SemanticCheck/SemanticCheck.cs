using IPFEngine.Parser;

namespace IPFEngine.SemanticCheck
{
    public class IPFSemanticCheck
    {
        static public IEnumerable<string> Check(IEnumerable<IPFVariable> IPFVars, IEnumerable<IPFFee> IPFFees)
        {
            // Number variables should not:
            // - have a MinValue greater than MaxValue
            // - have a DefaultValue less than MinValue or greater than MaxValue            
            var VarNumberErrors = IPFVars.OfType<IPFVariableNumber>().Where(w=>w.MinValue > w.MaxValue || w.DefaultValue < w.MinValue || w.DefaultValue > w.MaxValue).Select(s=>s.Name);
            foreach(var er in VarNumberErrors)
            {
                yield return string.Format("Invalid parameters for variable [{0}]. The Min value must not be greater than the Max value and the default value must be between Min and Max.", er);
            }

            // List variables should not:
            // - have duplicate symbols
            // - have the default value other than the defined symbols
            var VarList = IPFVars.OfType<IPFVariableList>();
            foreach (var vl in VarList)
            {
                vl.Values.Where(w=>Symbol.Distinct().Count() != w.Symbol.Count()).Select(s=>s.Name);
                yield return string.Format("Duplicate symbols for variable [{0}].", vl.Name);
            }
        }

        static private bool CheckVariable(IPFVariableList obj) => throw new NotImplementedException();

        static private bool CheckVariable(IPFVariableNumber obj) => throw new NotImplementedException();

        static private bool CheckVariable(IPFVariableBoolean obj) => throw new NotImplementedException();
    }
}