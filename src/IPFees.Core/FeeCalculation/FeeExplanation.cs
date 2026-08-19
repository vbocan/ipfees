namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Why a fee came out the way it did.
    ///
    /// The IPFLang engine records, for every yield it evaluates, the expression, the conditions
    /// guarding it, whether those conditions held, and the amount contributed. That record is
    /// what makes a computed fee auditable: a practitioner disputing an amount can see which
    /// rule fired and on what grounds, and a rule that did not fire is reported alongside the
    /// ones that did, since knowing why a discount was *not* applied is usually the question.
    ///
    /// These types restate the engine's provenance in a shape that serialises cleanly and does
    /// not oblige a client to reference the engine.
    /// </summary>
    /// <param name="TotalMandatory">Sum of the fees that are always incurred.</param>
    /// <param name="TotalOptional">Sum of the fees marked OPTIONAL in the schedule.</param>
    public record FeeExplanation(
        string FeeName,
        string FeeDescription,
        decimal TotalMandatory,
        decimal TotalOptional,
        IReadOnlyDictionary<string, string> Inputs,
        IReadOnlyList<ExplainedFee> Fees,
        IReadOnlyList<ExplainedAlternative> Alternatives)
    {
        public decimal GrandTotal => TotalMandatory + TotalOptional;
    }

    /// <param name="Steps">
    /// Every yield the engine evaluated for this fee, in the order it evaluated them, including
    /// those whose conditions did not hold.
    /// </param>
    public record ExplainedFee(
        string Name,
        bool Optional,
        decimal Amount,
        IReadOnlyList<ExplainedStep> Steps);

    /// <param name="Applied">
    /// False when the step was evaluated but its conditions did not hold, so it contributed
    /// nothing. These are retained deliberately: an absent charge is as much a part of the
    /// explanation as a present one.
    /// </param>
    /// <param name="Condition">The CASE and YIELD IF guards, as written in the schedule.</param>
    public record ExplainedStep(
        string Expression,
        decimal Contribution,
        bool Applied,
        string? Condition,
        IReadOnlyDictionary<string, string> ReferencedInputs,
        IReadOnlyDictionary<string, decimal> LocalVariables);

    /// <summary>
    /// What the total would have been had one input differed. Answers the question a
    /// practitioner asks after seeing a figure: what would it cost if we filed differently.
    /// </summary>
    public record ExplainedAlternative(
        string InputName,
        string OriginalValue,
        string AlternativeValue,
        decimal OriginalTotal,
        decimal AlternativeTotal,
        decimal Difference);
}
