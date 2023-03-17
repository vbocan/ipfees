using IPFees.Parser;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace IPFees.Evaluator
{

    /// <summary>
    /// Influenced by: https://github.com/toptensoftware/SimpleExpressionEngine
    /// https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce
    /// </summary>
    public class DslEvaluator
    {
        public DslEvaluator() { }

        private static readonly Dictionary<string, int> Operators = new() { { "LT", 4 }, { "LTE", 4 }, { "GT", 4 }, { "GTE", 4 }, { "EQ", 3 }, { "NEQ", 3 }, { "IN", 3 }, { "NIN", 3 }, { "AND", 2 }, { "OR", 1 } };

        public static double EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars) => EvaluateExpression(Tokens, Vars, string.Empty);
        public static double EvaluateExpression(string[] Tokens, IEnumerable<IPFValue> Vars, string FeeName)
        {
            return Parser.Parse(string.Join(" ", Tokens)).Eval(new MyContext(Vars, FeeName));
        }

        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars) => EvaluateLogic(Tokens, Vars, string.Empty);
        public static bool EvaluateLogic(string[] Tokens, IEnumerable<IPFValue> Vars, string FeeName)
        {
            // If no tokens are provided, the logic is implicitly true
            if (Tokens.Length == 0) return true;

            // Stack for values
            var values = new Stack<IPFValue>();
            // Stack for operators
            var ops = new Stack<string>();

            for (int i = 0; i < Tokens.Length; i++)
            {
                // Current token is a global variable                
                var Variable = Vars.SingleOrDefault(s => s.Name.Equals(Tokens[i]));
                if (Variable is not null)
                {
                    values.Push(Variable);
                    continue;
                }
                // Current token in a fee local variable
                var VarName = new StringBuilder().AppendFormat($"{Tokens[i]}.{FeeName}").ToString();
                var Variable2 = Vars.SingleOrDefault(s => s.Name.Equals(VarName));
                if (Variable2 is not null)
                {
                    values.Push(Variable2);
                    continue;
                }
                // Current token is an opening brace, push it to 'ops'
                if (Tokens[i].Equals("("))
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
            // Both values are booleans
            if (a is IPFValueBoolean && b is IPFValueBoolean)
            {
                var av = (a as IPFValueBoolean).Value;
                var bv = (b as IPFValueBoolean).Value;
                return op switch
                {
                    "AND" => new IPFValueBoolean(string.Empty, av && bv),
                    "OR" => new IPFValueBoolean(string.Empty, av || bv),
                    "EQ" => new IPFValueBoolean(string.Empty, av == bv),
                    "NEQ" => new IPFValueBoolean(string.Empty, av != bv),
                    "LT" or "LTE" or "GT" or "GTE" or "IN" or "NIN" => throw new NotSupportedException($"Operator [{op}] cannot compare booleans"),
                    _ => throw new InvalidDataException($"Unknown operator: '{op}'"),
                };
            }
            // Both values are string
            else if (a is IPFValueString && b is IPFValueString)
            {
                var av = (a as IPFValueString).Value;
                var bv = (b as IPFValueString).Value;
                return op switch
                {
                    "EQ" => new IPFValueBoolean(string.Empty, av == bv),
                    "NEQ" => new IPFValueBoolean(string.Empty, av != bv),
                    "AND" or "OR" or "LT" or "LTE" or "GT" or "GTE" or "IN" or "NIN" => throw new NotSupportedException($"Operator [{op}] cannot compare strings"),
                    _ => throw new InvalidDataException($"Unknown operator: '{op}'"),
                };
            }
            // Left value is string and right value is string list
            else if (a is IPFValueString && b is IPFValueStringList)
            {
                var av = (a as IPFValueString).Value;
                var bv = (b as IPFValueStringList).Value;
                return op switch
                {
                    "IN" => new IPFValueBoolean(string.Empty, bv.Contains(av)),
                    "NIN" => new IPFValueBoolean(string.Empty, !bv.Contains(av)),
                    "EQ" or "NEQ" or "AND" or "OR" or "LT" or "LTE" or "GT" or "GTE" => throw new NotSupportedException($"Operator [{op}] cannot compare string lists"),
                    _ => throw new InvalidDataException($"Unknown operator: '{op}'"),
                }; ;
            }
            // Both values are numeric
            else if (a is IPFValueNumber && b is IPFValueNumber)
            {
                var av = (a as IPFValueNumber).Value;
                var bv = (b as IPFValueNumber).Value;
                return op switch
                {
                    "EQ" => new IPFValueBoolean(string.Empty, av == bv),
                    "NEQ" => new IPFValueBoolean(string.Empty, av != bv),
                    "LT" => new IPFValueBoolean(string.Empty, av < bv),
                    "LTE" => new IPFValueBoolean(string.Empty, av <= bv),
                    "GT" => new IPFValueBoolean(string.Empty, av > bv),
                    "GTE" => new IPFValueBoolean(string.Empty, av >= bv),
                    "AND" or "OR" or "IN" or "NIN" => throw new NotSupportedException($"Operator [{op}] cannot compare numbers"),
                    _ => throw new InvalidDataException($"Unknown operator: '{op}'"),
                };
            }
            else throw new NotSupportedException($"Type mismatch for [{a}] and [{b}]");
        }

    }

    class MyContext : IContext
    {
        private readonly IEnumerable<IPFValue> Vars;
        private readonly string FeeName;
        public MyContext(IEnumerable<IPFValue> Vars, string FeeName)
        {
            this.Vars = Vars;
            this.FeeName = FeeName;
        }

        public double ResolveVariable(string name)
        {
            // Variable is global
            var v1 = Vars.OfType<IPFValueNumber>().Where(w => w.Name.Equals(name)).SingleOrDefault();
            if (v1 != null)
            {
                return v1.Value;
            }
            // Variable is local to the current fee
            var VarName = new StringBuilder().AppendFormat($"{FeeName}.{name}").ToString();
            var v2 = Vars.OfType<IPFValueNumber>().Where(w => w.Name.Equals(VarName)).SingleOrDefault();
            if (v2 != null)
            {
                return v2.Value;
            }
            // Variable is the COUNT property            
            if (name.EndsWith("_COUNT"))
            {
                // Determine the list name
                var ListName = name.Substring(0,name.Length - 6);
                // Get the list by its name
                var List = Vars.SingleOrDefault(s => s.Name.Equals(ListName)) as IPFValueStringList;
                // Push the length of the list (number of items)
                return List.Value.Count();
            }

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