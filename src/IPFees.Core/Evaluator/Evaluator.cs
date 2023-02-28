using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace IPFees.Evaluator
{

    /// <summary>
    /// Influenced by: https://github.com/toptensoftware/SimpleExpressionEngine
    /// https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce
    /// </summary>
    public class IPFEvaluator
    {
        public IPFEvaluator() { }

        private static readonly (string, int)[] Operators = new (string, int)[] { ("LT", 1), ("LTE", 1), ("GT", 1), ("GTE", 1), ("EQ", 2), ("NEQ", 2), ("AND", 3), ("OR", 4) };

        public static double EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            return Parser.Parse(string.Join(" ", Tokens)).Eval(new MyContext(Vars));
        }

        // Example: ClaimCount ABOVE 3 AND EntityType EQUALS NormalEntity
        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            // Stack for values
            var values = new Stack<IPFValue>();
            // Stack for operators
            var ops = new Stack<string>();

            for (int i = 0; i < Tokens.Length; i++)
            {
                // Search for a variable named the same as the current token
                var Variable = Vars.SingleOrDefault(s => s.Name.Equals(Tokens[i]));

                if (Variable is not null)
                {
                    values.Push(Variable);
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
                else if (Operators.Select(s => s.Item1).Contains(Tokens[i]))
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
                // Current token is a number
                else if (double.TryParse(Tokens[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double Number))
                {
                    values.Push(new IPFValueNumber(string.Empty, Number));
                }
                // Current token is a boolean (TRUE or FALSE)
                else if (bool.TryParse(Tokens[i], out bool Boolean))
                {
                    values.Push(new IPFValueBoolean(string.Empty, Boolean));
                }
                // Current token is not recognized
                else throw new NotSupportedException(string.Format("Invalid token encountered [{0}] while evaluating expression [{1}]", Tokens[i], string.Join(' ', Tokens)));
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
            var PrecOp1 = Operators.Where(w => w.Item1 == op1).Select(s => s.Item2).DefaultIfEmpty(int.MinValue).SingleOrDefault();
            var PrecOp2 = Operators.Where(w => w.Item1 == op2).Select(s => s.Item2).DefaultIfEmpty(int.MinValue).SingleOrDefault();
            if (PrecOp1 == int.MinValue) throw new InvalidDataException($"Unknown function: '{op1}'");
            if (PrecOp2 == int.MinValue) throw new InvalidDataException($"Unknown function: '{op2}'");
            return PrecOp2 > PrecOp1;
        }

        // A utility method to apply an operator 'op' on operands 'a' and 'b'. Return the result.
        private static bool ApplyOperation(string op, IPFValue b, IPFValue a)
        {
            switch (op)
            {
                case "AND":
                    return a + b;
                case "OR":
                    return a - b;
                case "LT":
                    return a * b;
                case "LTE":
                    return a * b;
                case "GT":
                    return a * b;
                case "GTE":
                    return a * b;
                case "EQ":
                    return a * b;
                case "NEQ":
                    return a * b;
            }
            return 0;
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