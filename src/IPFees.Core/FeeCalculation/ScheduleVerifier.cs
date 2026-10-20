using IPFLang.Analysis;
using IPFLang.Parser;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Runs a composed schedule's VERIFY directives one at a time against a single deadline.
    ///
    /// Running all directives in one engine call made the slowest one decide the outcome of
    /// every other: a monotonicity check that overran the budget discarded the completeness
    /// results that had already been computed, so a schedule with a coverage gap was saved as
    /// "unfinished" instead of being refused. Here completeness directives run first, since they
    /// settle in milliseconds, then each monotonicity directive in turn; whatever finished before
    /// the deadline is reported, and only the directives that did not are marked unfinished.
    ///
    /// A monotonicity check is given only the inputs its fee reads. The engine's checker samples
    /// every declared input to build the contexts in which it varies the target, and a schedule
    /// declares about sixteen, so the check could not finish on any schedule in the reference
    /// corpus. An input the fee never reads cannot change its amount, so leaving it out does not
    /// change the verdict, only the number of contexts examined.
    /// </summary>
    public class ScheduleVerifier
    {
        private readonly VerificationBudget Budget;

        public ScheduleVerifier(VerificationBudget? budget = null)
        {
            Budget = budget ?? new VerificationBudget();
        }

        public ScheduleVerification Verify(IEnumerable<DslInput> inputs, IEnumerable<DslFee> fees, IEnumerable<DslVerify> directives)
        {
            var allInputs = inputs.ToList();
            var feesByName = fees.GroupBy(f => f.Name).ToDictionary(g => g.Key, g => g.Last());
            var ordered = directives
                .OrderBy(d => d is DslVerifyComplete ? 0 : 1)
                .ToList();

            var result = new ScheduleVerification(ordered.Count);
            var deadline = DateTime.UtcNow + Budget.Limit;

            foreach (var directive in ordered)
            {
                if (!feesByName.TryGetValue(directive.FeeName, out var fee))
                {
                    result.Errors.Add($"VERIFY names fee '{directive.FeeName}', which this schedule does not define.");
                    continue;
                }

                var remaining = deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    result.Unfinished.Add(Describe(directive));
                    continue;
                }

                // The engine exposes no cancellation, so a check that overruns is abandoned rather
                // than stopped: it keeps a thread busy until it ends, but the caller gets an answer.
                var running = Task.Run(() => Run(directive, fee, allInputs));
                try
                {
                    if (!running.Wait(remaining))
                    {
                        result.Unfinished.Add(Describe(directive));
                        continue;
                    }
                }
                catch (AggregateException ex)
                {
                    result.Errors.Add($"Could not verify {Describe(directive)}: {ex.InnerException?.Message ?? ex.Message}");
                    continue;
                }

                switch (running.Result)
                {
                    case FeeCompletenessReport c: result.Completeness.Add(c); break;
                    case MonotonicityReport m: result.Monotonicity.Add(m); break;
                    case string error: result.Errors.Add(error); break;
                }
            }

            return result;
        }

        private static object Run(DslVerify directive, DslFee fee, IReadOnlyList<DslInput> inputs)
        {
            switch (directive)
            {
                case DslVerifyComplete:
                    var domains = new DomainAnalyzer().ExtractDomains(inputs).ToList();
                    return new CompletenessChecker().CheckFeeCompleteness(fee, domains);

                case DslVerifyMonotonic m:
                    if (!System.Enum.TryParse<MonotonicityDirection>(m.Direction, out var direction))
                    {
                        return $"VERIFY MONOTONIC on '{m.FeeName}' names an unknown direction '{m.Direction}'.";
                    }
                    var read = InputsReadBy(fee);
                    // A fee that never reads the input cannot change with it, so the directive
                    // would hold whatever the fee computes. It is refused rather than reported as
                    // passed: it most likely names the wrong input.
                    if (!read.Contains(m.WithRespectTo))
                    {
                        var candidates = read.Where(n => inputs.Any(i => i.Name == n)).OrderBy(n => n);
                        return $"VERIFY MONOTONIC on '{m.FeeName}' is with respect to '{m.WithRespectTo}', which the fee does not read, so it could never fail. The fee reads {string.Join(", ", candidates)}.";
                    }
                    var relevant = inputs.Where(i => i.Name == m.WithRespectTo || read.Contains(i.Name)).ToList();
                    return new MonotonicityChecker().CheckMonotonicity(fee, relevant, m.WithRespectTo, direction);

                default:
                    return $"Unsupported directive {directive}.";
            }
        }

        /// <summary>
        /// Names the fee's conditions, yields and LET bindings refer to. Postfix operators such as
        /// <c>ApplicationDate!MONTHSTONOW</c> are reduced to the input they apply to.
        /// </summary>
        public static HashSet<string> InputsReadBy(DslFee fee)
        {
            var names = new HashSet<string>();
            void Add(IEnumerable<string> tokens)
            {
                foreach (var t in tokens)
                {
                    var bang = t.IndexOf('!');
                    names.Add(bang > 0 ? t[..bang] : t);
                }
            }

            foreach (var let in fee.Vars) Add(let.ValueTokens);
            foreach (var item in fee.Cases)
            {
                Add(item.Condition);
                switch (item)
                {
                    case DslFeeCase c:
                        foreach (var y in c.Yields) { Add(y.Condition); Add(y.Values); }
                        break;
                    case DslFeeYield y:
                        Add(y.Values);
                        break;
                }
            }
            return names;
        }

        private static string Describe(DslVerify directive) => directive switch
        {
            DslVerifyComplete c => $"VERIFY COMPLETE FEE {c.FeeName}",
            DslVerifyMonotonic m => $"VERIFY MONOTONIC FEE {m.FeeName} WITH RESPECT TO {m.WithRespectTo}",
            _ => directive.ToString() ?? "VERIFY"
        };
    }

    /// <summary>What running a schedule's directives established, directive by directive.</summary>
    public class ScheduleVerification
    {
        public ScheduleVerification(int declared) { Declared = declared; }

        /// <summary>How many VERIFY directives the schedule declares.</summary>
        public int Declared { get; }
        public List<FeeCompletenessReport> Completeness { get; } = new();
        public List<MonotonicityReport> Monotonicity { get; } = new();

        /// <summary>Directives the deadline cut off: neither proven nor disproven.</summary>
        public List<string> Unfinished { get; } = new();

        /// <summary>Directives that could not be run at all, such as one naming an undefined fee.</summary>
        public List<string> Errors { get; } = new();

        public IEnumerable<FeeCompletenessReport> CompletenessFailures => Completeness.Where(r => !r.IsComplete);
        public IEnumerable<MonotonicityReport> MonotonicityFailures => Monotonicity.Where(r => !r.IsMonotonic);
    }
}
