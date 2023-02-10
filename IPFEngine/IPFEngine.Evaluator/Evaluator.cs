using IPFEngine.Parser;
using Microsoft.VisualBasic;
using System.Collections;

namespace IPFEngine.Evaluator
{
    public class IPFEvaluator
    {
        public IPFEvaluator() { }

        public static int EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            // Stack for numbers: 'values'
            var values = new Stack<int>();
            // Stack for Operators: 'ops'
            var ops = new Stack<string>();

            for (int i = 0; i < Tokens.Length; i++)
            {
                var Variable = Vars.SingleOrDefault(s => s.Name.Equals(Tokens[i]));

                // Current token is a number variable
                if (Variable is IPFValueNumber number)
                {
                    values.Push(number.Value);
                }
                // Current token is an opening brace, push it to 'ops'
                else if (Tokens[i].Equals("("))
                {
                    ops.Push(Tokens[i]);
                }
                // Closing brace encountered, solve entire brace
                else if (Tokens[i].Equals(")"))
                {
                    while (ops.Peek() != "(")
                    {
                        values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
                    }
                    ops.Pop();
                }
                // Current token is an operator.
                else if ((new string[] { "+", "-", "*", "/" }).Contains(Tokens[i]))
                {
                    // While top of 'ops' has same or greater precedence to current token, which is an operator.
                    // Apply operator on top of 'ops' to top two elements in values stack
                    while (ops.Count > 0 && HasPrecedence(Tokens[i], ops.Peek()))
                    {
                        values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
                    }

                    // Push current token to 'ops'.
                    ops.Push(Tokens[i]);
                }
                // Finally, it must be an integer
                else
                {
                    if (int.TryParse(Tokens[i], out int Number))
                    {
                        values.Push(Number);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Invalid token: {0}", Tokens[i]));
                    }
                }
            }

            // Entire expression has been parsed at this point, apply remaining ops to remaining values
            while (ops.Count > 0)
            {
                values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
            }

            // Top of 'values' contains result, return it
            return values.Pop();
        }

        // Returns true if 'op2' has higher or same precedence as 'op1', otherwise returns false.
        private static bool HasPrecedence(string op1, string op2)
        {
            if (op2 == "(" || op2 == ")")
            {
                return false;
            }
            if ((op1 == "*" || op1 == "/") && (op2 == "+" || op2 == "-"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // A utility method to apply an operator 'op' on operands 'a' and 'b'. Return the result.
        private static int ApplyOperation(string op, int b, int a)
        {
            switch (op)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    if (b == 0)
                    {
                        throw new NotSupportedException("Cannot divide by zero");
                    }
                    return (int)Math.Ceiling((double)a / b);
            }
            return 0;
        }

        private static bool EvaluateLogicItem(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            if (Tokens.Length <= 2) throw new NotSupportedException("Invalid logic");

            var CurrentVariable = Vars.SingleOrDefault(w => w.Name.Equals(Tokens[0]));
            if (CurrentVariable == null) throw new NotSupportedException(string.Format("Variable [{0}] was not found.", Tokens[0]));

            // The first token decides how we evaluate the item.
            // If the first token is a numeric variable, the available inequality operators are ABOVE, UNDER, EQUALS            
            if (CurrentVariable is IPFValueNumber number)
            {
                int LeftValue = number.Value;
                int RightValue = EvaluateExpression(Tokens.Skip(2).ToArray(), Vars);
                switch (Tokens[1])
                {
                    case "ABOVE": return LeftValue > RightValue;
                    case "BELOW": return LeftValue < RightValue;
                    case "EQUALS": return LeftValue == RightValue;
                    default: throw new NotSupportedException("Expected ABOVE, UNDER, EQUALS");
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
                    default: throw new NotSupportedException("Expected EQUALS");
                }
            }
            // If the first token is a boolean variable, there is only one operator available: EQUALS
            if (CurrentVariable is IPFValueBoolean boo)
            {
                bool LeftValue = boo.Value;
                var res = bool.TryParse(Tokens[2], out bool RightValue);
                if(!res) throw new NotSupportedException("Expected TRUE or FALSE");
                switch (Tokens[1])
                {
                    case "EQUALS": return LeftValue == RightValue;
                    default: throw new NotSupportedException("Expected EQUALS");
                }
            }
            return true;
        }

        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            // If there are no tokens, logic is false
            if (Tokens.Length == 0) return false;
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