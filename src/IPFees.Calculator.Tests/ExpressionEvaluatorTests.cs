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
            List<IPFValue> vars = new() {
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
            List<IPFValue> vars = new() {
                new IPFValueNumber("A", 10),
            };
            var tokens = "A * -1".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
            Assert.Equal(-10, ev);
        }

        [Fact]
        public void TestFunctionFloor()
        {
            List<IPFValue> vars = new() {
                new IPFValueNumber("A", 40.7),
            };
            var tokens = "FLOOR ( A )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
            Assert.Equal(40, ev);
        }

        [Fact]
        public void TestFunctionRound()
        {
            List<IPFValue> vars = new() {
                new IPFValueNumber("A", 40.3),
                new IPFValueNumber("B", 40.7),
            };
            var tokens1 = "ROUND ( A )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev1 = IPFEvaluator.EvaluateExpression(tokens1, vars);
            Assert.Equal(40, ev1);

            var tokens2 = "ROUND ( B )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev2 = IPFEvaluator.EvaluateExpression(tokens2, vars);
            Assert.Equal(41, ev2);
        }

        [Fact]
        public void TestFunctionCeil()
        {
            List<IPFValue> vars = new() {
                new IPFValueNumber("A", 40.3),
            };
            var tokens = "CEIL ( A )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
            Assert.Equal(41, ev);
        }
    }
}