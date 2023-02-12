using IPFEngine.Parser;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace IPFEngine.Tests
{
    public class FeeTests
    {
        [Fact]
        public void TestFeeWithYields()
        {
            string text =
            """
            COMPUTE FEE BasicNationalFee
            YIELD 320 IF EntityType IS NormalEntity
            YIELD 128 IF EntityType IS SmallEntity 
            YIELD 64 IF EntityType IS MicroEntity
            ENDCOMPUTE
            """;
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = p.GetFees().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("BasicNationalFee", result.Name);                        
            Assert.Equal(1, result.Cases.Count);
            var FeeCase = result.Cases[0];
            Assert.IsType<IPFFeeCase>(FeeCase);
            var FeeCase1 = (IPFFeeCase)result.Cases[0];
            Assert.Empty(FeeCase1.Condition);
            Assert.Equal(3, FeeCase1.Yields.Count);
        }

        [Fact]
        public void TestFeeWithCases()
        {
            string text =
            """
            COMPUTE FEE SearchFee
            CASE SituationType IS PreparedIPEA AS
            	YIELD 0 IF EntityType IS NormalEntity
            	YIELD 0 IF EntityType IS SmallEntity
            	YIELD 0 IF EntityType IS MicroEntity
            ENDCASE
            CASE SituationType IS PaidAsISA OR SituationType IS DeliveredAsISA AS
            	YIELD 140 IF EntityType IS NormalEntity
            	YIELD 56 IF EntityType IS SmallEntity            	
            ENDCASE
            CASE SituationType IS PreparedISA AS
            	YIELD 540 IF EntityType IS NormalEntity            	            	
            ENDCASE            
            ENDCOMPUTE
            """;
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = p.GetFees().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("SearchFee", result.Name);
            Assert.Equal(3, result.Cases.Count);
            var FeeCase = result.Cases[0];
            Assert.IsType<IPFFeeCase>(FeeCase);
            var FeeCase1 = (IPFFeeCase)result.Cases[0];
            Assert.Equal(3, FeeCase1.Condition.Count());
            Assert.Equal(3, FeeCase1.Yields.Count);
            var FeeCase2 = (IPFFeeCase)result.Cases[1];
            Assert.Equal(7, FeeCase2.Condition.Count());
            Assert.Equal(2, FeeCase2.Yields.Count);
            var FeeCase3 = (IPFFeeCase)result.Cases[2];
            Assert.Equal(3, FeeCase3.Condition.Count());
            Assert.Equal(1, FeeCase3.Yields.Count);
        }

        [Fact]
        public void TestFeeWithCasesAndYields()
        {
            string text =
            """
            COMPUTE FEE ExaminationFee
            CASE SituationType IS PreparedIPEA AS
            	YIELD 0 IF EntityType IS NormalEntity
            	YIELD 0 IF EntityType IS SmallEntity
            	YIELD 0 IF EntityType IS MicroEntity
            ENDCASE
            YIELD 800 IF EntityType IS NormalEntity
            YIELD 360 IF EntityType IS SmallEntity
            YIELD 160 IF EntityType IS MicroEntity
            ENDCOMPUTE
            """;
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = p.GetFees().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ExaminationFee", result.Name);
            Assert.Equal(2, result.Cases.Count);
            var FeeCase = result.Cases[0];
            Assert.IsType<IPFFeeCase>(FeeCase);
            var FeeCase1 = (IPFFeeCase)result.Cases[0];
            Assert.Equal(3, FeeCase1.Condition.Count());
            Assert.Equal(3, FeeCase1.Yields.Count);
            var FeeCase2 = (IPFFeeCase)result.Cases[1];
            Assert.Empty(FeeCase2.Condition);
            Assert.Equal(3, FeeCase2.Yields.Count);            
        }
    }
}