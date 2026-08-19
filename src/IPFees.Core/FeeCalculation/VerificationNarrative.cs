using IPFLang.Analysis;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Turns IPFLang's verification reports into sentences a fee author can act on.
    /// Shared by the web editor and the REST API so both describe a failure the same way.
    /// </summary>
    public static class VerificationNarrative
    {
        private const int MaxExamples = 3;

        /// <summary>Explain why a fee failed its completeness check.</summary>
        public static string Describe(FeeCompletenessReport report)
        {
            var summary = $"'{report.FeeName}' does not cover every input combination " +
                          $"({report.Gaps.Count} uncovered, checked by {report.VerificationMethod}).";

            if (report.Gaps.Count == 0) return summary;

            var examples = report.Gaps.Take(MaxExamples).Select(g => g.ToString());
            var more = report.Gaps.Count > MaxExamples ? $", and {report.Gaps.Count - MaxExamples} more" : string.Empty;
            return $"{summary} For example: {string.Join("; ", examples)}{more}.";
        }

        /// <summary>Explain why a fee failed its monotonicity check.</summary>
        public static string Describe(MonotonicityReport report)
        {
            var summary = $"'{report.FeeName}' is not {report.ExpectedDirection} with respect to " +
                          $"'{report.WithRespectTo}' ({report.Violations.Count} violations).";

            var first = report.Violations.FirstOrDefault();
            return first is null ? summary : $"{summary} For example: {first}.";
        }
    }
}
