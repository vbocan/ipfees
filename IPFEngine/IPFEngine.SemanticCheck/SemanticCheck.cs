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
            var VarNumberErrors = IPFVars.OfType<IPFVariableNumber>().Where(w => w.MinValue > w.MaxValue || w.DefaultValue < w.MinValue || w.DefaultValue > w.MaxValue).Select(s => s.Name);
            foreach (var er in VarNumberErrors)
            {
                yield return string.Format("Invalid parameters for variable [{0}]. The Min value must not be greater than the Max value and the default value must be between Min and Max.", er);
            }

            // List variables should not:
            // - have duplicate symbols
            // - have the default value other than the defined symbols
            // - have the same symbol defined in multiple variables 
            var VarList = IPFVars.OfType<IPFVariableList>();
            foreach (var er in VarList.Where(vl => vl.Items.Count != vl.Items.Select(s => s.Symbol).Distinct().Count()).Select(s => s.Name))
            {
                yield return string.Format("Duplicate symbols at variable [{0}].", er);
            }
            foreach (var er in VarList.Where(w => !w.Items.Select(s => s.Symbol).Contains(w.DefaultSymbol)).Select(s => s.Name))
            {
                yield return string.Format("Default value should be one of the symbols at variable [{0}].", er);
            }            
            foreach (var er in VarList.SelectMany(s => s.Items.DistinctBy(e => e.Symbol)).Select(a => a.Symbol).GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key))
            {
                yield return string.Format("Symbol [{0}] is defined in multiple variables.", er);
            }
        }
    }
}