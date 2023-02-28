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

        private static readonly Dictionary<string, int> Operators = new() { { "LT", 4 }, { "LTE", 4 }, { "GT", 4 }, { "GTE", 4 }, { "EQ", 3 }, { "NEQ", 3 }, { "AND", 2 }, { "OR", 1 } };

        public static double EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            return Parser.Parse(string.Join(" ", Tokens)).Eval(new MyContext(Vars));
        }

        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars)
        {
            // If no tokens are provided, the logic is implicitly true
            if (Tokens.Length == 0) return true;

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
                else if (Operators.ContainsKey(Tokens[i]))
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
                // Current token is a string
                else
                {
                    values.Push(new IPFValueString(string.Empty, Tokens[i]));
                }
            }

            // Entire expression has been parsed at this point, apply remaining ops to remaining values
            while (ops.Count > 0)
            {
                values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
            }

            // Top of 'values' contains result, return it
            var result = values.Pop();
            if (result is not IPFValueBoolean) throw new NotSupportedException(string.Format("Invalid logic expression: [{0}]", string.Join(' ', Tokens)));
            return (result as IPFValueBoolean).Value;
        }

        // Returns true if 'op2' has higher or same precedence as 'op1', otherwise returns false.
        private static bool HasPrecedence(string op1, string op2)
        {
            if (op2 == "(" || op2 == ")")
            {
                return false;
            }
            if (!Operators.ContainsKey(op1)) throw new InvalidDataException($"Unknown operator: '{op1}'");
            if (!Operators.ContainsKey(op2)) throw new InvalidDataException($"Unknown operator: '{op2}'");
            return Operators[op2] > Operators[op1];
        }

        // A utility method to apply an operator 'op' on operands 'a' and 'b'. Return the result.
        private static IPFValueBoolean ApplyOperation(string op, IPFValue b, IPFValue a)
        {
            switch (op)
            {
                case "AND":
                    return new IPFValueBoolean(string.Empty, (a as IPFValueBoolean).Value && (b as IPFValueBoolean).Value);

                case "OR": return new IPFValueBoolean(string.Empty, (a as IPFValueBoolean).Value || (b as IPFValueBoolean).Value);
                case "LT":
                    return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value < (b as IPFValueNumber).Value);
                case "LTE":
                    return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value <= (b as IPFValueNumber).Value);
                case "GT":
                    return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value > (b as IPFValueNumber).Value);
                case "GTE":
                    return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value >= (b as IPFValueNumber).Value);
                case "EQ":
                    if (b is IPFValueNumber && a is IPFValueNumber)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value == (b as IPFValueNumber).Value);
                    }
                    else if (b is IPFValueString && a is IPFValueString)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueString).Value == (b as IPFValueString).Value);
                    }
                    else if (b is IPFValueBoolean && a is IPFValueBoolean)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueBoolean).Value == (b as IPFValueBoolean).Value);
                    }
                    else throw new NotSupportedException($"[{a}] and [{b}] must have the same type (string, boolean, numeric)");
                case "NEQ":
                    if (b is IPFValueNumber && a is IPFValueNumber)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueNumber).Value != (b as IPFValueNumber).Value);
                    }
                    else if (b is IPFValueString && a is IPFValueString)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueString).Value != (b as IPFValueString).Value);
                    }
                    else if (b is IPFValueBoolean && a is IPFValueBoolean)
                    {
                        return new IPFValueBoolean(string.Empty, (a as IPFValueBoolean).Value != (b as IPFValueBoolean).Value);
                    }
                    else throw new NotSupportedException($"[{a}] and [{b}] must have the same type (string, boolean, numeric)");
                default: throw new InvalidDataException($"Unknown operator: '{op}'");
            };
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