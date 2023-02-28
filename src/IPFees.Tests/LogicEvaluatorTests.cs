using IPFees.Evaluator;
using IPFees.Parser;
using Newtonsoft.Json.Linq;

namespace IPFees.Tests
{
    public class LogicEvaluatorTests
    {
        [Fact]
        public void TestNumericEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 6),                
            };
            var tokens = "A EQ 6".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestNumericNotEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 7),
            };
            var tokens = "A NEQ 6".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestBooleanEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueBoolean("B", true),
            };
            var tokens = "B EQ TRUE".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestBooleanNotEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueBoolean("B", false),
            };
            var tokens = "B NEQ TRUE".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestStringEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
            };
            var tokens = "EntityType EQ NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestStringNotEqual()
        {
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NewEntity"),
            };
            var tokens = "EntityType NEQ NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }
        
        [Fact]
        public void TestNumbersAnd()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 100),
                new IPFValueNumber("B", 200),
                new IPFValueNumber("C", 300),
            };
            var tokens = "A LT B AND B LT C".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestBooleanOrWithParentheses()
        {
            var vars = new IPFValue[] {
                new IPFValueBoolean("A", true),
                new IPFValueBoolean("B", false),                
            };
            var tokens = "( A OR B )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestStringNumberAnd()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 100),
                new IPFValueNumber("B", 200),
                new IPFValueString("C", "Alibaba"),
            };
            var tokens = "A LT B AND C EQ Alibaba".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestNumbersOr()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 100),
                new IPFValueNumber("B", 200),                
            };
            var tokens = "A GT 1000 OR 50 LT B".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestNumbersAndOrWithParentheses()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 100),
                new IPFValueNumber("B", 200),
                new IPFValueNumber("C", 30),
            };
            var tokens = "A LT 1000 AND ( 50 GTE B OR C EQ 30 )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }

        [Fact]
        public void TestMultipleWithParentheses()
        {
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 100),
                new IPFValueNumber("B", 200),
                new IPFValueNumber("C", 30),
                new IPFValueBoolean("bo", true),
                new IPFValueString("str", "MyString")
            };
            var tokens = "( A GTE 100 AND B EQ 200 ) OR ( C NEQ 30 AND bo EQ TRUE AND str EQ MyString )".Split(new char[] { ' ' }, StringSplitOptions.None);
            var ev = IPFEvaluator.EvaluateLogic(tokens, vars);
            Assert.True(ev);
        }
    }
}