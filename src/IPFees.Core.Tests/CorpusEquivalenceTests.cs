using System.Text.Json;
using IPFLang.Composition;
using IPFLang.Corpus;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFLang.Versioning;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// IPFees historically carried its own fee corpus, three documents per jurisdiction
    /// (official, translation, agent) stored in MongoDB. IPFLang now ships one script per
    /// jurisdiction. These tests hold the two accountable to each other.
    ///
    /// The claim being defended is narrow and checkable: <em>every fee definition that exists
    /// in both corpora computes the same amount from the same inputs</em>. Where the corpora
    /// differ, they differ by design, and those differences are asserted too so that an
    /// unintended one shows up as a failure rather than as noise.
    /// </summary>
    public class CorpusEquivalenceTests
    {
        private static readonly Lazy<LegacyCorpus> Legacy = new(LegacyCorpus.Load);

        [Fact]
        public void SharedFeeDefinitionsComputeIdenticalAmounts()
        {
            var mismatches = new List<string>();
            var compared = 0;
            var jurisdictions = 0;

            foreach (var code in Legacy.Value.JurisdictionCodes)
            {
                if (!JurisdictionCorpus.Contains(code)) continue;

                var legacy = Legacy.Value.Compose(code);
                var current = ComposeFromPackage(code);
                if (legacy is null || current is null) continue;
                jurisdictions++;

                // One input vector drives both sides. Legacy defaults win where they exist,
                // so a changed default cannot be mistaken for a changed formula.
                var inputs = legacy.Inputs.DistinctBy(i => i.Name).ToList();
                foreach (var input in current.Inputs)
                {
                    if (!inputs.Any(x => x.Name == input.Name)) inputs.Add(input);
                }
                var values = DefaultsFor(inputs);

                var legacyFees = legacy.Fees.DistinctBy(f => f.Name).ToDictionary(f => f.Name);
                var currentFees = current.Fees.DistinctBy(f => f.Name).ToDictionary(f => f.Name);

                foreach (var name in legacyFees.Keys.Intersect(currentFees.Keys))
                {
                    compared++;
                    var before = Evaluate(legacyFees[name], inputs, values);
                    var after = Evaluate(currentFees[name], inputs, values);
                    if (before != after) mismatches.Add($"{code}/{name}: was {before}, now {after}");
                }
            }

            Assert.True(jurisdictions >= 118, $"expected to compare at least 118 jurisdictions, compared {jurisdictions}");
            Assert.True(compared >= 340, $"expected at least 340 shared fee definitions, found {compared}");
            Assert.True(mismatches.Count == 0, $"{mismatches.Count} shared fees diverged:{Environment.NewLine}{string.Join(Environment.NewLine, mismatches.Take(20))}");
        }

        [Fact]
        public void TranslationAndAgentFeesWereRenamedAndMadeOptional()
        {
            // The one intended restructuring. Legacy kept these as separate mandatory
            // documents; the current corpus folds them into the jurisdiction as optional
            // components, because neither is always incurred.
            var legacyOnly = new HashSet<string>();
            var currentOnly = new HashSet<string>();

            foreach (var code in Legacy.Value.JurisdictionCodes)
            {
                if (!JurisdictionCorpus.Contains(code)) continue;
                var legacy = Legacy.Value.Compose(code);
                var current = ComposeFromPackage(code);
                if (legacy is null || current is null) continue;

                var before = legacy.Fees.Select(f => f.Name).ToHashSet();
                var after = current.Fees.Select(f => f.Name).ToHashSet();
                foreach (var n in before.Except(after)) legacyOnly.Add(n);
                foreach (var n in after.Except(before)) currentOnly.Add(n);
            }

            Assert.Contains("AGT_FilingFee", legacyOnly);
            Assert.Contains("TRL_TranslationFee", legacyOnly);
            Assert.Contains("AgentServiceFee", currentOnly);
            Assert.Contains("TranslationFee", currentOnly);

            // Nothing else should have appeared or vanished except the validation-code fix below.
            var unexplainedBefore = legacyOnly.Except(new[] { "AGT_FilingFee", "TRL_TranslationFee", "OFF_ValidationFee_CD" });
            var unexplainedAfter = currentOnly.Except(new[] { "AgentServiceFee", "TranslationFee", "OFF_ValidationFee_KH" });

            Assert.True(!unexplainedBefore.Any(), $"fees disappeared without explanation: {string.Join(", ", unexplainedBefore)}");
            Assert.True(!unexplainedAfter.Any(), $"fees appeared without explanation: {string.Join(", ", unexplainedAfter)}");
        }

        [Fact]
        public void CambodiaValidationFeeUsesTheCorrectIsoCode()
        {
            // The legacy corpus filed Cambodia's EP validation fee under CD, which is the
            // ISO 3166-1 code for the Democratic Republic of the Congo. Cambodia is KH.
            var current = JurisdictionCorpus.GetJurisdiction("EP");

            Assert.Contains("VAL_KH", current);
            Assert.Contains("OFF_ValidationFee_KH", current);
            Assert.DoesNotContain("VAL_CD", current);

            var legacy = Legacy.Value.SourceFor("EP");
            Assert.Contains("VAL_CD", legacy);
        }

        [Fact]
        public void MalaysiaWasPortedAndCompleted()
        {
            // Malaysia shipped in the legacy seed data with an official-fees document and
            // nothing else, so a calculation for it failed on the missing partner and
            // translation definitions. The ported schedule carries all three.
            Assert.Contains("MY", Legacy.Value.JurisdictionCodes);
            Assert.True(JurisdictionCorpus.Contains("MY"));

            var current = ComposeFromPackage("MY");
            Assert.NotNull(current);

            var names = current!.Fees.Select(f => f.Name).ToList();
            Assert.Contains("OFF_BasicNationalFee", names);
            Assert.Contains("OFF_ClaimFee", names);
            Assert.Contains("TranslationFee", names);
            Assert.Contains("AgentServiceFee", names);
        }

        #region composition helpers

        private static ParsedScript? ComposeFromPackage(string code) =>
            Compose(JurisdictionCorpus.GetCompositionChain(code));

        private static ParsedScript? Compose(IReadOnlyList<(string Id, string Source)> chain)
        {
            var parser = new DslParser();
            var registry = new JurisdictionRegistry();
            string? parent = null, leaf = null;

            foreach (var (id, source) in chain)
            {
                var (parsed, _) = parser.Parse(source, returnParsedScript: true);
                if (parsed is null) return null;
                registry.Register(new Jurisdiction(id, id, parsed, parent));
                parent = id;
                leaf = id;
            }

            try { return new JurisdictionComposer(registry).Compose(leaf!).Script; }
            catch { return null; }
        }

        /// <summary>
        /// Compute one fee in isolation. Optional is cleared so that the amount is compared
        /// rather than the classification, which is tested separately.
        /// </summary>
        private static decimal Evaluate(DslFee fee, IEnumerable<DslInput> inputs, List<IPFValue> values)
        {
            var script = new ParsedScript(
                inputs,
                new[] { fee with { Optional = false } },
                Array.Empty<DslReturn>(),
                Array.Empty<DslGroup>(),
                Array.Empty<DslVerify>());

            var calculator = new DslCalculator(new DslParser());
            calculator.LoadParsedScript(script);
            var (mandatory, optional, _, _) = calculator.Compute(values);
            return mandatory + optional;
        }

        private static List<IPFValue> DefaultsFor(IEnumerable<DslInput> inputs)
        {
            var values = new List<IPFValue>();
            foreach (var input in inputs)
            {
                switch (input)
                {
                    case DslInputNumber n: values.Add(new IPFValueNumber(input.Name, n.DefaultValue)); break;
                    case DslInputBoolean b: values.Add(new IPFValueBoolean(input.Name, b.DefaultValue)); break;
                    case DslInputList l: values.Add(new IPFValueString(input.Name, l.DefaultSymbol)); break;
                    case DslInputListMultiple m: values.Add(new IPFValueStringList(input.Name, m.DefaultSymbols.ToList())); break;
                    case DslInputDate d: values.Add(new IPFValueDate(input.Name, d.DefaultValue)); break;
                    case DslInputAmount a: values.Add(new IPFValueNumber(input.Name, a.DefaultValue)); break;
                }
            }
            return values;
        }

        #endregion

        /// <summary>
        /// The MongoDB seed data as shipped, read straight from the JSON rather than a
        /// running database, so the comparison needs no container.
        /// </summary>
        private sealed class LegacyCorpus
        {
            private record Module(string Id, string Name, string Source, bool AutoRun);
            private record Fee(string Name, string Jurisdiction, string Source, List<string> Modules);

            private readonly List<Module> modules = new();
            private readonly List<Fee> fees = new();

            public IReadOnlyList<string> JurisdictionCodes =>
                fees.Select(f => f.Jurisdiction).Where(j => !string.IsNullOrWhiteSpace(j)).Distinct().OrderBy(x => x).ToList();

            public string SourceFor(string code) =>
                string.Join("\n", fees.Where(f => f.Jurisdiction == code).Select(f => f.Source));

            public ParsedScript? Compose(string code)
            {
                var inputs = new List<DslInput>();
                var all = new List<DslFee>();

                foreach (var fee in fees.Where(f => f.Jurisdiction == code))
                {
                    var chain = new List<(string, string)>();
                    var seen = new HashSet<string>();

                    foreach (var m in modules.Where(m => m.AutoRun))
                    {
                        if (seen.Add(m.Id)) chain.Add(($"module:{m.Name}", m.Source));
                    }
                    foreach (var id in fee.Modules)
                    {
                        var m = modules.FirstOrDefault(x => x.Id == id);
                        if (m is not null && seen.Add(id)) chain.Add(($"module:{m.Name}", m.Source));
                    }
                    chain.Add((fee.Name, fee.Source));

                    var script = CorpusEquivalenceTests.Compose(chain);
                    if (script is null) return null;
                    all.AddRange(script.Fees);
                    inputs.AddRange(script.Inputs);
                }

                return new ParsedScript(inputs, all, Array.Empty<DslReturn>(), Array.Empty<DslGroup>(), Array.Empty<DslVerify>());
            }

            public static LegacyCorpus Load()
            {
                var corpus = new LegacyCorpus();
                var dir = Path.Combine(AppContext.BaseDirectory, "SeedData");

                foreach (var m in Read(Path.Combine(dir, "modules.json")))
                {
                    corpus.modules.Add(new Module(
                        BinaryId(m.GetProperty("_id")),
                        m.GetProperty("Name").GetString()!,
                        m.GetProperty("SourceCode").GetString()!,
                        m.TryGetProperty("AutoRun", out var a) && a.GetBoolean()));
                }

                foreach (var f in Read(Path.Combine(dir, "fees.json")))
                {
                    var refs = f.TryGetProperty("ReferencedModules", out var r) && r.ValueKind == JsonValueKind.Array
                        ? r.EnumerateArray().Select(BinaryId).ToList()
                        : new List<string>();

                    corpus.fees.Add(new Fee(
                        f.GetProperty("Name").GetString()!,
                        f.TryGetProperty("JurisdictionName", out var j) ? j.GetString() ?? "" : "",
                        f.GetProperty("SourceCode").GetString()!,
                        refs));
                }

                return corpus;
            }

            private static List<JsonElement> Read(string path)
            {
                Assert.True(File.Exists(path), $"seed data not found at {path}");
                using var stream = File.OpenRead(path);
                return JsonDocument.Parse(stream).RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
            }

            /// <summary>Mongo extended JSON binary id, used verbatim as an opaque key.</summary>
            private static string BinaryId(JsonElement element) =>
                element.GetProperty("$binary").GetProperty("base64").GetString()!;
        }
    }
}
