using IPFEngine.Parser;
using Newtonsoft.Json.Linq;

namespace IPFEngine.Tests
{
    public class ParserTest
    {
        [Fact]
        public void TestVariableList()
        {
            string text =
            """
            # Define a list variable
            DEFINE LIST EntityType AS 'Entity type'
            VALUE 'Normal' AS NormalEntity
            VALUE 'Small' AS SmallEntity
            VALUE 'Micro' AS MicroEntity
            DEFAULT NormalEntity
            END
            """;
            var p = new IPFParser(text);
            var result = (IPFVariableList?)p.Parse().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("EntityType", result.Name);
            Assert.Equal("Entity type", result.Text);
            Assert.Equal("NormalEntity", result.DefaultValue);
            Assert.Equal(3, result.Values.Count);
            Assert.Equal(new IPFListItem("NormalEntity", "Normal"), result.Values[0]);
            Assert.Equal(new IPFListItem("SmallEntity", "Small"), result.Values[1]);
            Assert.Equal(new IPFListItem("MicroEntity", "Micro"), result.Values[2]);
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
            END
            """;
            var p = new IPFParser(text);
            var result = (IPFVariableNumber?)p.Parse().SingleOrDefault();
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
            END
            """;
            var p = new IPFParser(text);
            var result = (IPFVariableBoolean?)p.Parse().SingleOrDefault();
            Assert.NotNull(result);
            Assert.Equal("ContainsDependentClaims", result.Name);
            Assert.Equal("Contains dependent claims", result.Text);
            Assert.True(result.DefaultValue);
        }
    }
}