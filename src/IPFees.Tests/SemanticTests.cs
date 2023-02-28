using IPFees.Parser;

namespace IPFees.Tests
{
    public class SemanticTests
    {
        [Fact]
        public void TestListNoError()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            CHOICE NormalEntity AS 'Normal'
            CHOICE SmallEntity AS 'Small'
            CHOICE MicroEntity AS 'Micro'
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.True(result);
            var errors = p.GetErrors();
            Assert.Empty(errors);
        }

        [Fact]
        public void TestListDuplicateChoice()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            CHOICE NormalEntity AS 'Normal'
            CHOICE NormalEntity AS 'Small'
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False (result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a =>a.Item1 == IPFError.VariableDuplicateChoices);
        }

        [Fact]
        public void TestListWrongDefaultChoice()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            CHOICE 'Normal' AS NormalEntity
            CHOICE 'Small' AS SmallEntity            
            DEFAULT TinyEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.VariableInvalidDefaultChoice);
        }

        [Fact]
        public void TestListWrongDefaultAndDuplicateChoice()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            CHOICE 'Normal' AS NormalEntity
            CHOICE 'Small' AS NormalEntity            
            DEFAULT TinyEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 is IPFError.VariableDuplicateChoices or IPFError.VariableInvalidDefaultChoice);
        }

        [Fact]
        public void TestListNoChoices()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.VariableNoChoice);
        }

        [Fact]
        public void TestListDuplicateChoicesInMultipleVariables()
        {
            string text =
            """
            DEFINE LIST EntityType1 AS 'Select the desired entity type'
            CHOICE 'Normal' AS NormalEntity            
            DEFAULT NormalEntity
            ENDDEFINE

            DEFINE LIST EntityType2 AS 'Select the desired entity type'
            CHOICE 'Normal' AS NormalEntity            
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.ChoiceDefinedInMultipleVariables);
        }

        [Fact]
        public void TestNumberMinMax()
        {
            string text =
            """
            DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
            BETWEEN 10 AND 1000
            DEFAULT 15
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.True(result);            
        }

        [Fact]
        public void TestNumberMinOverMax()
        {
            string text =
            """
            DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
            BETWEEN 20 AND 10
            DEFAULT 15
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.InvalidMinMaxDefault);
        }

        [Fact]
        public void TestNumberDefaultOutOfRange()
        {
            string text =
            """
            DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
            BETWEEN 10 AND 20
            DEFAULT 21
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.InvalidMinMaxDefault);
        }

        [Fact]
        public void TestVariableDefinedMultipleTimes()
        {
            string text =
            """
            DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
            BETWEEN 10 AND 20
            DEFAULT 11
            ENDDEFINE
            DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
            BETWEEN 10 AND 20
            DEFAULT 11
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.VariableDefinedMultipleTimes);
        }

        [Fact]
        public void TestFeeDefinedMultipleTimes()
        {
            string text =
            """
            COMPUTE FEE SheetFee
            YIELD 420 * SheetCount / 50 IF SheetCount OVER 100 AND EntityType IS NormalEntity
            YIELD 168 * SheetCount / 50 IF SheetCount OVER 100 AND EntityType IS SmallEntity
            YIELD 84 * SheetCount / 50 IF SheetCount OVER 100 AND EntityType IS MicroEntity
            ENDCOMPUTE
            COMPUTE FEE SheetFee
            YIELD 480 * ClaimCount IF ClaimCount OVER 3 AND EntityType IS NormalEntity
            YIELD 192 IF ClaimCount OVER 3 AND EntityType IS SmallEntity
            YIELD 96 IF ClaimCount OVER 3 AND EntityType IS MicroEntity
            ENDCOMPUTE
            """;
            var p = new IPFParser(text);
            var result = p.Parse();
            Assert.False(result);
            var errors = p.GetErrors();
            Assert.Contains(errors, a => a.Item1 == IPFError.FeeDefinedMultipleTimes);
        }
    }
}