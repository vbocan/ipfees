using IPFees.Calculator;
using IPFees.Evaluator;

namespace IPFees.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void TestEmptyFee()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            ENDCOMPUTE
            """;
            var vars = Array.Empty<IPFValue>();

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(0, TotalAmount);
        }

        [Fact]
        public void TestSimpleYield()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            YIELD 320
            YIELD 80
            ENDCOMPUTE
            """;
            var vars = Array.Empty<IPFValue>();

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(400, TotalAmount);
        }

        [Fact]
        public void TestYieldWithSimpleCondition()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            YIELD 320 IF TRUE
            YIELD 80
            ENDCOMPUTE
            """;
            var vars = Array.Empty<IPFValue>();

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(400, TotalAmount);
        }

        [Fact]
        public void TestYieldWithComplexCondition()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            YIELD 320 IF EntityType EQUALS NormalEntity
            YIELD 128 IF EntityType EQUALS SmallEntity 
            YIELD 64 IF EntityType EQUALS MicroEntity
            ENDCOMPUTE
            """;
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
            };

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(320, TotalAmount);
        }

        [Fact]
        public void TestComplexYieldWithComplexCondition()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            YIELD (320/10-30)*2 IF EntityType EQUALS NormalEntity
            YIELD (128/5) IF EntityType EQUALS SmallEntity 
            YIELD (64/8)+2 IF EntityType EQUALS MicroEntity
            ENDCOMPUTE
            """;
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
            };

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(4, TotalAmount);
        }

        [Fact]
        public void TestYieldWithVariable()
        {
            string text =
            """
            DEFINE NUMBER A AS 'A number'
            ENDDEFINE

            COMPUTE FEE BasicNationalFee
            YIELD (A/10-30)*2
            ENDCOMPUTE
            """;
            var vars = new IPFValue[] {
                new IPFValueNumber("A", 350),
            };

            var calc = new IPFCalculator();
            calc.Parse(text);
            var (TotalAmount, _) = calc.Compute(vars);

            Assert.Equal(10, TotalAmount);
        }
    }
}