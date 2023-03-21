using System.Text;

namespace IPFees.Evaluator
{
    class EvaluatorContext : IContext
    {
        private readonly IEnumerable<IPFValue> Vars;
        private readonly string FeeName;
        public EvaluatorContext(IEnumerable<IPFValue> Vars, string FeeName)
        {
            this.Vars = Vars;
            this.FeeName = FeeName;
        }

        public double ResolveVariable(string name)
        {
            #region Global variable
            var v1 = Vars.OfType<IPFValueNumber>().Where(w => w.Name.Equals(name)).SingleOrDefault();
            if (v1 != null)
            {
                return v1.Value;
            }
            #endregion
            #region Fee local variable            
            var VarName = new StringBuilder().AppendFormat($"{FeeName}.{name}").ToString();
            var v2 = Vars.OfType<IPFValueNumber>().Where(w => w.Name.Equals(VarName)).SingleOrDefault();
            if (v2 != null)
            {
                return v2.Value;
            }
            #endregion
            #region COUNT property of a multiple selection list
            if (name.EndsWith("!COUNT"))
            {
                // Determine the list name
                var ListName = name[..^6];
                // Get the list by its name
                var List = Vars.SingleOrDefault(s => s.Name.Equals(ListName)) as IPFValueStringList;
                // Push the length of the list (number of items)
                return List.Value.Count();
            }
            #endregion
            #region YEARSTONOW property of a date
            if (name.EndsWith("!YEARSTONOW"))
            {
                // Determine date name
                var DateName = name[..^11];
                // Get the date by its name
                var Date = Vars.SingleOrDefault(s => s.Name.Equals(DateName)) as IPFValueDate;
                // Push the difference between dates as years
                TimeSpan difference = DateTime.Now.Subtract(Date.Value.ToDateTime(TimeOnly.MinValue));
                return difference.TotalDays / 365.25;
            }
            #endregion
            #region MONTHSTONOW property of a date
            if (name.EndsWith("!MONTHSTONOW"))
            {
                // Determine date name
                var DateName = name[..^12];
                // Get the date by its name
                var Date = Vars.SingleOrDefault(s => s.Name.Equals(DateName)) as IPFValueDate;
                // Push the difference between dates as months
                TimeSpan difference = DateTime.Now.Subtract(Date.Value.ToDateTime(TimeOnly.MinValue));
                return difference.TotalDays / (365.25 / 12);
            }
            #endregion
            #region DAYSTONOW property of a date
            if (name.EndsWith("!DAYSTONOW"))
            {
                // Determine date name
                var DateName = name[..^10];
                // Get the date by its name
                var Date = Vars.SingleOrDefault(s => s.Name.Equals(DateName)) as IPFValueDate;
                // Push the difference between dates as days
                TimeSpan difference = DateTime.Now.Subtract(Date.Value.ToDateTime(TimeOnly.MinValue));
                return difference.TotalDays;
            }
            #endregion
            throw new InvalidDataException($"No variable [{name}] in fee [{FeeName}]'");
        }

        public double CallFunction(string name, double[] arguments)
        {
            return name switch
            {
                "ROUND" => Math.Round(arguments[0]),
                "FLOOR" => Math.Floor(arguments[0]),
                "CEIL" => Math.Ceiling(arguments[0]),
                _ => throw new InvalidDataException($"Unknown function: '{name}'"),
            };
        }
    }
}