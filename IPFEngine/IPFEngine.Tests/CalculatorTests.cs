using IPFEngine.Calculator;
using IPFEngine.Evaluator;
using IPFEngine.Parser;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace IPFEngine.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void Test1()
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

            var calc = new IPFCalculator(text);
            var (TotalAmount, _) = calc.Compute(vars);
            
            Assert.Equal(320, TotalAmount);
        }
    }
}