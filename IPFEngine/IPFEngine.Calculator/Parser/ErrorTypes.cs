namespace IPFees.Parser
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
