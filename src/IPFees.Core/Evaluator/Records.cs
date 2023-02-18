using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFees.Evaluator
{
    public abstract record IPFValue(string Name);
    public record IPFValueString(string Name, string Value) : IPFValue(Name)
    {
        public override string ToString()
        {
            return string.Format("String: [{0}]", Value);
        }
    }
    public record IPFValueNumber(string Name, int Value) : IPFValue(Name)
    {
        public override string ToString()
        {
            return string.Format("Number: [{0}]", Value);
        }
    };
    public record IPFValueBoolean(string Name, bool Value) : IPFValue(Name)
    {
        public override string ToString()
        {
            return string.Format("Boolean: [{0}]", Value);
        }
    };
}
