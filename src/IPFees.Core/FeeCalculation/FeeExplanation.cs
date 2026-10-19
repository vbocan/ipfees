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
    /// <param name="Id">
    /// The fee document's own id, so a caller can link straight to its IPFLang source (the
    /// Fee/Edit page) without a second lookup by name.
    /// </param>
    /// <param name="Totals">
    /// The fees that are always incurred and those marked OPTIONAL, added up per currency. A
    /// schedule whose official fees are in CHF and whose translation is in EUR has two totals; the
    /// engine's own totals add such amounts as bare numbers and are not reported.
    /// </param>
    public record FeeExplanation(
        Guid Id,
        string FeeName,
        string FeeDescription,
        IReadOnlyList<CurrencyTotal> Totals,
        IReadOnlyDictionary<string, string> Inputs,
        IReadOnlyList<ExplainedFee> Fees,
        IReadOnlyList<ExplainedAlternative> Alternatives);

    /// <param name="Steps">
    /// Every yield the engine evaluated for this fee, in the order it evaluated them, including
    /// those whose conditions did not hold.
    /// </param>
    /// <param name="Currency">
    /// The currency <paramref name="Amount"/> is in, which need not be the schedule's: an agent's
    /// charge written as <c>100&lt;EUR&gt;</c> is 100 EUR in a schedule that otherwise prices in SGD.
    /// </param>
    public record ExplainedFee(
        string Name,
        bool Optional,
        decimal Amount,
        IReadOnlyList<ExplainedStep> Steps,
        string? Currency = null);

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
    /// What the totals would have been had one input differed. Answers the question a
    /// practitioner asks after seeing a figure: what would it cost if we filed differently.
    /// The totals are per currency, as the explanation's own are, so raising the claim count of a
    /// schedule priced in CHF and EUR moves the CHF total and leaves the EUR total as it was. An
    /// alternative is reported only when the mandatory or the optional total of some currency
    /// changes.
    /// </summary>
    /// <param name="OriginalTotals">The totals with the inputs as given, the same as the explanation's.</param>
    /// <param name="AlternativeTotals">
    /// The totals with <paramref name="InputName"/> set to <paramref name="AlternativeValue"/> and
    /// every other input as given.
    /// </param>
    /// <param name="Differences">
    /// Alternative minus original for each currency, in the same order as the totals; a currency
    /// the change does not reach is listed with zeros.
    /// </param>
    public record ExplainedAlternative(
        string InputName,
        string OriginalValue,
        string AlternativeValue,
        IReadOnlyList<CurrencyTotal> OriginalTotals,
        IReadOnlyList<CurrencyTotal> AlternativeTotals,
        IReadOnlyList<CurrencyTotal> Differences);
}
