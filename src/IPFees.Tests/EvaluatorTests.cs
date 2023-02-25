using IPFees.Evaluator;
using IPFees.Parser;
using Newtonsoft.Json.Linq;

namespace IPFees.Tests
{
    public class EvaluatorTests
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

        [Fact]
        public void TestEqualsEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 6),                
            };
            var tokens = "A EQUALS 60 / 10".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestNotEqualsEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 7),
            };
            var tokens = "A NOTEQUALS 60 / 10".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestStringEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
            };
            var tokens = "EntityType EQUALS NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestLogicEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
                new IPFValueNumber("SheetCount", 101),
            };
            var tokens = "SheetCount ABOVE 100 AND EntityType EQUALS NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestBooleanEvaluation()
        {
            var vars = new IPFValue[] {
                new IPFValueBoolean("ContainsDependentClaims", true),                
            };
            var tokens = "ContainsDependentClaims EQUALS TRUE".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }
    }
}