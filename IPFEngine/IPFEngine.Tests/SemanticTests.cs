using IPFEngine.Parser;

namespace IPFEngine.Tests
{
    public class SemanticTests
    {
        [Fact]
        public void TestListNoError()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity
            VALUE 'Small' AS SmallEntity
            VALUE 'Micro' AS MicroEntity
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);                       
            Assert.Empty(ck);
        }

        [Fact]
        public void TestListDuplicateSymbol()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity
            VALUE 'Small' AS NormalEntity            
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Single(ck);
        }

        [Fact]
        public void TestListWrongDefaultSymbol()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity
            VALUE 'Small' AS SmallEntity            
            DEFAULT TinyEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Single(ck);
        }

        [Fact]
        public void TestListWrongDefaultAndDuplicateSymbol()
        {
            string text =
            """
            DEFINE LIST EntityType AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity
            VALUE 'Small' AS NormalEntity            
            DEFAULT TinyEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Equal(2, ck.Count());
        }

        [Fact]
        public void TestListDuplicateSymbolInMultipleVariables()
        {
            string text =
            """
            DEFINE LIST EntityType1 AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity            
            DEFAULT NormalEntity
            ENDDEFINE

            DEFINE LIST EntityType2 AS 'Select the desired entity type'
            VALUE 'Normal' AS NormalEntity            
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new IPFParser(text);
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Single(ck);
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
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);            
            Assert.Empty(ck);
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
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Single(ck);            
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
            var (vars, fees) = p.Parse();
            var ck = IPFSemanticChecker.Check(vars, fees);
            Assert.Single(ck);
        }
    }
}