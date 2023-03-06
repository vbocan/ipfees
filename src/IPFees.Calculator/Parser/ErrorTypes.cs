namespace IPFees.Parser
{
    public enum IPFError
    {
        None,
        SyntaxError,
        VariableDefinedMultipleTimes,
        FeeDefinedMultipleTimes,
        FeeVariableDefinedMultipleTimes,
        InvalidMinMaxDefault,
        VariableNoChoice,
        VariableNoDefaultChoice,
        VariableDuplicateChoices,
        VariableInvalidDefaultChoice,
        ChoiceDefinedInMultipleVariables
    }
}
