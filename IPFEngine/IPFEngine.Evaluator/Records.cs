using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFEngine.Evaluator
{
    public abstract record IPFValue(string Name);
    public record IPFValueBoolean(string Name, bool Value) : IPFValue(Name);
    public record IPFValueList(string Name, string Value) : IPFValue(Name);
    public record IPFValueNumber(string Name, int Value) : IPFValue(Name);    
}
