using IPFees.Parser;

namespace IPFees.Calculator.Tests
{
    public class VariableTests
    {
        [Fact]
        public void TestVariableList()
        {
            string text =
            """
            # Define a list variable
            DEFINE LIST EntityType AS 'Entity type'
            CHOICE NormalEntity AS 'Normal'
            CHOICE SmallEntity AS 'Small'
            CHOICE MicroEntity AS 'Micro'
            DEFAULT NormalEntity
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableList?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("EntityType", result.Name);
            Assert.Equal("Entity type", result.Text);
            Assert.Equal("NormalEntity", result.DefaultSymbol);            
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(new DslListItem("NormalEntity", "Normal"), result.Items[0]);
            Assert.Equal(new DslListItem("SmallEntity", "Small"), result.Items[1]);
            Assert.Equal(new DslListItem("MicroEntity", "Micro"), result.Items[2]);
        }

        [Fact]
        public void TestVariableListMultiple()
        {
            string text =
            """
            # Define a multiple-selection list variable
            DEFINE LIST EntityType AS 'Entity type' MULTIPLE
            CHOICE NormalEntity AS 'Normal'
            CHOICE SmallEntity AS 'Small'
            CHOICE MicroEntity AS 'Micro'
            DEFAULT SmallEntity,NormalEntity
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableListMultiple?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("EntityType", result.Name);
            Assert.Equal("Entity type", result.Text);
            Assert.Equal("SmallEntity", result.DefaultSymbols[0]);
            Assert.Equal("NormalEntity", result.DefaultSymbols[1]);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(new DslListItem("NormalEntity", "Normal"), result.Items[0]);
            Assert.Equal(new DslListItem("SmallEntity", "Small"), result.Items[1]);
            Assert.Equal(new DslListItem("MicroEntity", "Micro"), result.Items[2]);
        }

        [Fact]
        public void TestCommentInString()
        {
            string text =
            """
            # Define a list variable
            DEFINE LIST L1 AS 'List #1'
            CHOICE C1 AS 'Choice #1'
            DEFAULT C1
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableList?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("L1", result.Name);
            Assert.Equal("List #1", result.Text);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal(new DslListItem("C1", "Choice #1"), result.Items[0]);
        }

        [Fact]
        public void TestVariableNumber()
        {
            string text =
            """
            # Define a number variable
            DEFINE NUMBER ClaimCount AS 'Number of claims'
            BETWEEN 0 AND 1000
            DEFAULT 0
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableNumber?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ClaimCount", result.Name);
            Assert.Equal("Number of claims", result.Text);
            Assert.Equal(0, result.MinValue);
            Assert.Equal(1000, result.MaxValue);
            Assert.Equal(0, result.DefaultValue);
        }

        [Fact]
        public void TestVariableBoolean()
        {
            string text =
            """
            # Define a boolean variable
            DEFINE BOOLEAN ContainsDependentClaims AS 'Contains dependent claims'
            DEFAULT TRUE
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableBoolean?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ContainsDependentClaims", result.Name);
            Assert.Equal("Contains dependent claims", result.Text);
            Assert.True(result.DefaultValue);
        }

        [Fact]
        public void TestVariableDate()
        {
            string text =
            """
            # Define a date variable
            DEFINE DATE ApplicationDate AS 'Application date'
            BETWEEN 2023-01-01 AND $TODAY
            DEFAULT 2023-03-01
            ENDDEFINE
            """;
            var p = new DslParser();
            var _ = p.Parse(text);
            var result = (DslVariableDate?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ApplicationDate", result.Name);
            Assert.Equal("Application date", result.Text);
            Assert.Equal(new DateTime(2023, 3, 1), result.DefaultValue);
            Assert.Equal(new DateTime(2023, 1, 1), result.MinValue);
            Assert.Equal(DateTime.Now.Date, result.MaxValue);            
        }        
    }
}