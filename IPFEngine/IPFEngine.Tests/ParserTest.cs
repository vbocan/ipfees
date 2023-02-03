using IPFEngine.Parser;

namespace IPFEngine.Tests
{
    public class ParserTest
    {
        [Fact]
        public void TestVariableList()
        {
            string[] ipf_data = File.ReadAllLines(@"..\..\..\input_list.txt");
            var p = new IPFParser(ipf_data);
            var result = (IPFVariableList)p.Parse().Single();
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
            string[] ipf_data = File.ReadAllLines(@"..\..\..\input_number.txt");
            var p = new IPFParser(ipf_data);
            var result = (IPFVariableNumber)p.Parse().Single();
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
            string[] ipf_data = File.ReadAllLines(@"..\..\..\input_boolean.txt");
            var p = new IPFParser(ipf_data);
            var result = (IPFVariableBoolean)p.Parse().Single();
            Assert.NotNull(result);
            Assert.Equal("ContainsDependentClaims", result.Name);
            Assert.Equal("Contains dependent claims", result.Text);
            Assert.True(result.DefaultValue);
        }        
    }
}