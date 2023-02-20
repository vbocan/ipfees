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
            return string.Format("[{0}] = String: [{1}]", Name, Value);
        }
    }
    public record IPFValueNumber(string Name, double Value) : IPFValue(Name)
    {
        public override string ToString()
        {
            return string.Format("[{0}] = Number: [{1}]", Name, Value);
        }
    };
    public record IPFValueBoolean(string Name, bool Value) : IPFValue(Name)
    {
        public override string ToString()
        {
            return string.Format("[{0}] = Boolean: [{1}]", Name, Value);
        }
    };
}
