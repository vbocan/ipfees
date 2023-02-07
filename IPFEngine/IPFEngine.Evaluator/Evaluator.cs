using System.Text;

namespace IPFEngine.Evaluator
{
    public class IPFEvaluator
    {
        public IPFEvaluator()
        {
            var tokens = "10 + 2 * 6".Split(new char[] {' '}, StringSplitOptions.None);
            Console.WriteLine(EvaluateTokens(tokens));            
        }

        public static int EvaluateTokens(string[] tokens)
        {
            // Stack for numbers: 'values'
            Stack<int> values = new Stack<int>();
            // Stack for Operators: 'ops'
            Stack<string> ops = new Stack<string>();

            for (int i = 0; i < tokens.Length; i++)
            {
                // Current token is a number, push it to stack for numbers
                int Number = int.MinValue;
                var IsNumber = int.TryParse(tokens[i], out Number);
                if (IsNumber)
                {
                    values.Push(Number);
                }
                // Current token is an opening brace, push it to 'ops'
                else if (tokens[i].Equals("("))
                {
                    ops.Push(tokens[i]);
                }
                // Closing brace encountered, solve entire brace
                else if (tokens[i].Equals(")"))
                {
                    while (ops.Peek() != "(")
                    {
                        values.Push(applyOp(ops.Pop(), values.Pop(), values.Pop()));
                    }
                    ops.Pop();
                }

                // Current token is an operator.
                else if ((new string[] { "+", "-", "*", "/" }).Contains(tokens[i]))
                {
                    // While top of 'ops' has same or greater precedence to current token, which is an operator.
                    // Apply operator on top of 'ops' to top two elements in values stack
                    while (ops.Count > 0 && hasPrecedence(tokens[i], ops.Peek()))
                    {
                        values.Push(applyOp(ops.Pop(), values.Pop(), values.Pop()));
                    }

                    // Push current token to 'ops'.
                    ops.Push(tokens[i]);
                }
            }

            // Entire expression has been parsed at this point, apply remaining ops to remaining values
            while (ops.Count > 0)
            {
                values.Push(applyOp(ops.Pop(), values.Pop(), values.Pop()));
            }

            // Top of 'values' contains result, return it
            return values.Pop();
        }

        // Returns true if 'op2' has higher or same precedence as 'op1', otherwise returns false.
        public static bool hasPrecedence(string op1, string op2)
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
        public static int applyOp(string op, int b, int a)
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
                        throw new System.NotSupportedException("Cannot divide by zero");
                    }
                    return a / b;
            }
            return 0;
        }
    }
}