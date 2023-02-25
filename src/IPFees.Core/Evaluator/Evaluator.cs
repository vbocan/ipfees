namespace IPFees.Evaluator
{

    /// <summary>
    /// Influenced by: https://github.com/toptensoftware/SimpleExpressionEngine
    /// https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce
    /// </summary>
    public class IPFEvaluator
    {
        public IPFEvaluator() { }

        public static double EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            return Parser.Parse(string.Join(" ", Tokens)).Eval(new MyContext(Vars));
        }

        private static bool EvaluateLogicItem(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            if (Tokens.Length < 1) throw new NotSupportedException(string.Format("[{0}] is not valid logic", string.Join(' ', Tokens)));

            // Case 1: We have a <boolean value> in the form of TRUE or FALSE
            var IsPlainBoolean = bool.TryParse(Tokens[0], out bool PlainBooleanValue);
            if (IsPlainBoolean) return PlainBooleanValue;


            var CurrentVariable = Vars.SingleOrDefault(w => w.Name.Equals(Tokens[0]));
            if (CurrentVariable == null) throw new NotSupportedException(string.Format("Variable [{0}] was not found.", Tokens[0]));

            // Case 2: We have a <variable> followed by one of the operators (ABOVE, BELOW, EQUALS)
            // The first token decides how we evaluate the item.

            // If the first token is a boolean variable, there is only one operator available: EQUALS
            if (CurrentVariable is IPFValueBoolean boo)
            {
                if (Tokens.Length > 1)
                {
                    // Expression is in the form <variable> EQUALS TRUE|FALSE
                    bool LeftValue = boo.Value;
                    var res = bool.TryParse(Tokens[2], out bool RightValue);
                    if (!res) throw new NotSupportedException("Expected TRUE or FALSE");
                    switch (Tokens[1])
                    {
                        case "EQUALS": return LeftValue == RightValue;
                        case "NOTEQUALS": return LeftValue != RightValue;
                        default: throw new NotSupportedException("Expected EQUALS or NOTEQUALS");
                    }
                }
                else
                {
                    // Expression contains only the <variable> name
                    return boo.Value;
                }
            }

            // If the first token is a numeric variable, the available inequality operators are ABOVE, UNDER, EQUALS            
            if (CurrentVariable is IPFValueNumber number)
            {
                double LeftValue = number.Value;
                double RightValue = EvaluateExpression(Tokens.Skip(2).ToArray(), Vars);
                switch (Tokens[1])
                {
                    case "ABOVE": return LeftValue > RightValue;
                    case "BELOW": return LeftValue < RightValue;
                    case "EQUALS": return LeftValue == RightValue;
                    case "NOTEQUALS": return LeftValue != RightValue;
                    default: throw new NotSupportedException("Expected ABOVE, UNDER, EQUALS, NOTEQUALS");
                }
            }
            // If the first token is a string variable, there is only one operator available: EQUALS
            if (CurrentVariable is IPFValueString str)
            {
                string LeftValue = str.Value;
                string RightValue = Tokens[2];
                switch (Tokens[1])
                {
                    case "EQUALS": return LeftValue.Equals(RightValue);
                    case "NOTEQUALS": return !LeftValue.Equals(RightValue);
                    default: throw new NotSupportedException("Expected EQUALS or NOTEQUALS");
                }
            }

            return true;
        }

        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            // If there are no tokens, logic is true
            if (Tokens.Length == 0) return true;
            // Split token list at AND boundaries
            var AndItems = Tokens.Split("AND");

            bool result = true;
            foreach (var item in AndItems)
            {
                result = result && EvaluateLogicItem(item.ToArray(), Vars);
            }

            return result;
        }

    }

    class MyContext : IContext
    {
        private readonly IEnumerable<IPFValue> Vars;
        public MyContext(IEnumerable<IPFValue> Vars)
        {
            this.Vars = Vars;
        }

        public double ResolveVariable(string name)
        {
            var v = Vars.OfType<IPFValueNumber>().Where(w => w.Name.Equals(name)).SingleOrDefault();
            return v != null ? v.Value : throw new InvalidDataException($"Unknown variable: '{name}'");
        }

        public double CallFunction(string name, double[] arguments)
        {
            switch (name)
            {
                case "ROUND":
                    return Math.Round(arguments[0]);
                case "FLOOR":
                    return Math.Floor(arguments[0]);
                case "CEIL":
                    return Math.Ceiling(arguments[0]);
            }

            throw new InvalidDataException($"Unknown function: '{name}'");
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<IEnumerable<string>> Split(this string[] strings, string delimiter)
        {
            var start = 0;
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i] == delimiter)
                {
                    yield return strings.Skip(start).Take(i - start);
                    start = i + 1;
                }
            }

            yield return strings.Skip(start);
        }
    }
}