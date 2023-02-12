using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFEngine.Parser
{
    public enum IPFError
    {
        None,
        SyntaxError,
        VariableDefinedMultipleTimes,
        FeeDefinedMultipleTimes,
        InvalidMinMaxDefault,
        VariableNoChoice,
        VariableNoDefaultChoice,
        VariableDuplicateChoices,
        VariableInvalidDefaultChoice,
        ChoiceDefinedInMultipleVariables
    }
}
