using IPFees.Evaluator;
using IPFees.Parser;
using Newtonsoft.Json.Linq;

namespace IPFees.Tests
{
    public class ExpressionEvaluatorTests
    {
        [Fact]
        public void TestArithmeticEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 6),
                new IPFValueNumber("B", 80),
                new IPFValueNumber("C", 30),
            };
            var tokens = "( B + 2 ) * C - 11 * ( B + 2 * A )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
            Assert.Equal(1448, ev);
        }

        [Fact]
        public void TestArithmeticEvaluationWithNegativeNumber()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 10),
            };
            var tokens = "A * -1".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
            Assert.Equal(-10, ev);
        }
    }
}