using IPFees.Parser;
using Newtonsoft.Json.Linq;

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
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = (IPFVariableList?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("EntityType", result.Name);
            Assert.Equal("Entity type", result.Text);
            Assert.Equal("NormalEntity", result.DefaultSymbol);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(new IPFListItem("NormalEntity", "Normal"), result.Items[0]);
            Assert.Equal(new IPFListItem("SmallEntity", "Small"), result.Items[1]);
            Assert.Equal(new IPFListItem("MicroEntity", "Micro"), result.Items[2]);
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
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = (IPFVariableNumber?)p.GetVariables().SingleOrDefault();
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
            var p = new IPFParser(text);
            var _ = p.Parse();
            var result = (IPFVariableBoolean?)p.GetVariables().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ContainsDependentClaims", result.Name);
            Assert.Equal("Contains dependent claims", result.Text);
            Assert.True(result.DefaultValue);
        }
    }
}