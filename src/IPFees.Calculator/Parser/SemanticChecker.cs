namespace IPFees.Parser
{
    class IPFSemanticChecker
    {
        static public IEnumerable<(IPFError, string)> Check(IEnumerable<IPFVariable> IPFVars, IEnumerable<IPFFee> IPFFees)
        {
            // Variables should not:
            // - have duplicate names
            foreach (var er in IPFVars.Select(s => s.Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key))
            {
                yield return (IPFError.VariableDefinedMultipleTimes, string.Format("Variable [{0}] has multiple definitions.", er));
            }

            // Fees should not:
            // - have duplicate names
            // - have duplicate variables
            foreach (var er in IPFFees.Select(s => s.Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key))
            {
                yield return (IPFError.FeeDefinedMultipleTimes, string.Format("Fee [{0}] has multiple definitions.", er));
            }
            foreach (var er in IPFFees)
            {
                foreach (var dup in er.Vars.Select(s => s.Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).Distinct())
                {
                    yield return (IPFError.FeeVariableDefinedMultipleTimes, string.Format("Variable [{0}] has multiple definitions.", dup));
                }
            }

            // Number variables should not:
            // - have a MinValue greater than MaxValue
            // - have a DefaultValue less than MinValue or greater than MaxValue            
            var VarNumberErrors = IPFVars.OfType<IPFVariableNumber>().Where(w => w.MinValue > w.MaxValue || w.DefaultValue < w.MinValue || w.DefaultValue > w.MaxValue).Select(s => s.Name);
            foreach (var er in VarNumberErrors)
            {
                yield return (IPFError.InvalidMinMaxDefault, string.Format("Invalid parameters for variable [{0}]. The Min value must not be greater than the Max value and the default value must be between Min and Max.", er));
            }

            // List variables should not:
            // - have zero choices
            // - have no default choice
            // - have duplicate choices
            // - have the default choice other than the defined choices
            // - have the same choice defined in multiple variables
            var VarList = IPFVars.OfType<IPFVariableList>();
            foreach (var er in VarList.Where(vl => vl.Items.Count == 0).Select(s => s.Name))
            {
                yield return (IPFError.VariableNoChoice, string.Format("Variable [{0}] has no associated choices.", er));
            }
            foreach (var er in VarList.Where(vl => string.IsNullOrEmpty(vl.DefaultSymbol)).Select(s => s.Name))
            {
                yield return (IPFError.VariableNoDefaultChoice, string.Format("Variable [{0}] has no default choice.", er));
            }
            foreach (var er in VarList.Where(vl => vl.Items.Count != vl.Items.Select(s => s.Symbol).Distinct().Count()).Select(s => s.Name))
            {
                yield return (IPFError.VariableDuplicateChoices, string.Format("Variable [{0}] has duplicate choices.", er));
            }
            foreach (var er in VarList.Where(w => !w.Items.Select(s => s.Symbol).Contains(w.DefaultSymbol)).Select(s => s.Name))
            {
                yield return (IPFError.VariableInvalidDefaultChoice, string.Format("Default choice for variable [{0}] is invalid.", er));
            }            
            foreach (var er in VarList.SelectMany(s => s.Items.DistinctBy(e => e.Symbol)).Select(a => a.Symbol).GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key))
            {
                yield return (IPFError.ChoiceDefinedInMultipleVariables, string.Format("Choice [{0}] is defined in multiple variables.", er));
            }            
        }
    }
}