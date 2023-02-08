using System.Collections;

namespace IPFEngine.Evaluator
{
    public class IPFEvaluator
    {
        public IPFEvaluator() { }

        public static int EvaluateTokens(string[] Tokens, Hashtable Vars)
        {
            // Stack for numbers: 'values'
            Stack<int> values = new Stack<int>();
            // Stack for Operators: 'ops'
            Stack<string> ops = new Stack<string>();

            for (int i = 0; i < Tokens.Length; i++)
            {
                // Current token is a number, push it to stack for numbers
                int Number = int.MinValue;
                var IsNumber = int.TryParse(Tokens[i], out Number);
                if (IsNumber)
                {
                    values.Push(Number);
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
        public static bool HasPrecedence(string op1, string op2)
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
        public static int ApplyOperation(string op, int b, int a)
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