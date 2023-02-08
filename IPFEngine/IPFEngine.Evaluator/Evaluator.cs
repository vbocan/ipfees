using Microsoft.VisualBasic;
using System.Collections;

namespace IPFEngine.Evaluator
{
    public class IPFEvaluator
    {
        public IPFEvaluator() { }

        public static int EvaluateExpression(string[] Tokens, IDictionary<string, string> Vars)
        {
            // Stack for numbers: 'values'
            var values = new Stack<int>();
            // Stack for Operators: 'ops'
            var ops = new Stack<string>();

            for (int i = 0; i < Tokens.Length; i++)
            {
                var IsVariable = Vars.ContainsKey(Tokens[i]);

                // Current token is a variable
                if (IsVariable)
                {
                    // Try to convert the variable value to an integer                    
                    if (int.TryParse(Vars[Tokens[i]], out int VariableValue))
                    {
                        values.Push(VariableValue);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Variable {0} has an invalid value.", Tokens[i]));
                    }
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
                    return a / b;
            }
            return 0;
        }

        public static bool EvaluateInequality(string[] Tokens, IDictionary<string, string> Vars)
        {
            // Count the number of ABOVE tokens
            var AboveCount = Tokens.Where(w => w.Equals("ABOVE")).Count();
            if (AboveCount > 1) throw new NotSupportedException("Only one ABOVE operand is allowed in expression.");
            // Count the number of BELOW tokens
            var BelowCount = Tokens.Where(w => w.Equals("BELOW")).Count();
            if (BelowCount > 1) throw new NotSupportedException("Only one BELOW operand is allowed in expression.");
            // Both ABOVE and BELOW cannot appear in the same expression
            if (AboveCount + BelowCount > 1) throw new NotSupportedException("Either ABOVE or BELOW (not both) can appear in expression.");
            var NewTokens = Tokens.Select(s => s.Replace("ABOVE", "-").Replace("BELOW", "-")).ToArray();

            var result = EvaluateExpression(NewTokens, Vars);            
            return (AboveCount == 1) ? result >= 0 : result <= 0;
        }

        //public static bool EvaluateLogic(string[] Tokens, IDictionary<string, string> Vars)
        //{
        //    // Split token list at OR boundaries
        //    var OrExpressions = Tokens.Split("OR");

        //    foreach (var splitString in splitStrings)
        //    {
        //        Console.WriteLine("-->");
        //        Console.WriteLine(string.Join(",", splitString));
        //    }

        //}
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