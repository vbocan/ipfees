using System.Text.RegularExpressions;
using IPFees.Core.Enum;
using IPFees.Core.Repository;
using IPFLang.Corpus;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Loads the jurisdiction corpus that ships inside the IPFLang package into the database.
    ///
    /// The corpus is the reference fee schedule set, versioned with the language rather than
    /// with this application, so the schedules a deployment serves move forward when the
    /// package does. Regional bases become modules and the jurisdictions that extend them
    /// reference those modules, which is how the composition chain gets rebuilt at run time.
    /// </summary>
    public interface ICorpusSeeder
    {
        Task<CorpusSeedReport> SeedAsync(CancellationToken cancellationToken = default);
    }

    public record CorpusSeedReport(int Jurisdictions, int Bases, IReadOnlyList<string> Errors)
    {
        public bool Succeeded => Errors.Count == 0;
    }

    /// <inheritdoc cref="ICorpusSeeder"/>
    public class CorpusSeeder : ICorpusSeeder
    {
        private readonly IJurisdictionRepository jurisdictions;
        private readonly IFeeRepository fees;
        private readonly IModuleRepository modules;

        public CorpusSeeder(IJurisdictionRepository jurisdictions, IFeeRepository fees, IModuleRepository modules)
        {
            this.jurisdictions = jurisdictions;
            this.fees = fees;
            this.modules = modules;
        }

        public async Task<CorpusSeedReport> SeedAsync(CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();

            // Regional bases first: a jurisdiction cannot reference one that is not there yet.
            var baseIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in JurisdictionCorpus.BaseNames)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    baseIds[name] = await UpsertModule(name, JurisdictionCorpus.GetBase(name));
                }
                catch (Exception ex)
                {
                    errors.Add($"base '{name}': {ex.Message}");
                }
            }

            var seeded = 0;
            foreach (var code in JurisdictionCorpus.Codes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var source = JurisdictionCorpus.GetJurisdiction(code);
                    var description = DescriptionOf(source, code);

                    await UpsertJurisdiction(code, description);

                    var referenced = new List<Guid>();
                    var baseName = JurisdictionCorpus.GetBaseNameFor(code);
                    if (baseName is not null && baseIds.TryGetValue(baseName, out var baseId))
                    {
                        referenced.Add(baseId);
                    }

                    await UpsertFee(code, description, source, referenced);
                    seeded++;
                }
                catch (Exception ex)
                {
                    errors.Add($"jurisdiction '{code}': {ex.Message}");
                }
            }

            return new CorpusSeedReport(seeded, baseIds.Count, errors);
        }

        /// <summary>
        /// A schedule states its own name through RETURN JurisdictionName; the header comment is
        /// the fallback for one that does not.
        /// </summary>
        private static string DescriptionOf(string source, string code)
        {
            var declared = Regex.Match(source, @"RETURN\s+JurisdictionName\s+AS\s+'(?<text>[^']+)'", RegexOptions.IgnoreCase);
            if (declared.Success) return declared.Groups["text"].Value;

            var described = Regex.Match(source, @"DESCRIPTION\s+'(?<text>[^']+)'", RegexOptions.IgnoreCase);
            return described.Success ? described.Groups["text"].Value : $"PCT national phase: {code}";
        }

        private async Task<Guid> UpsertModule(string name, string source)
        {
            var existing = (await modules.GetModules()).FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            Guid id;

            if (existing is null)
            {
                var added = await modules.AddModuleAsync(name);
                if (!added.Success) throw new InvalidOperationException(added.Reason);
                id = added.Id;
            }
            else
            {
                id = existing.Id;
            }

            await modules.SetModuleDescriptionAsync(id, $"IPFLang regional base '{name}'");
            await modules.SetModuleSourceCodeAsync(id, source);

            // Bases apply only to the jurisdictions that name them, never to every calculation.
            await modules.SetModuleAutoRunStatusAsync(id, false);
            return id;
        }

        private async Task UpsertJurisdiction(string code, string description)
        {
            var existing = (await jurisdictions.GetJurisdictions()).FirstOrDefault(j => j.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                var added = await jurisdictions.AddJurisdictionAsync(code);
                if (!added.Success) throw new InvalidOperationException(added.Reason);
                await jurisdictions.SetJurisdictionServiceFeeLevelAsync(added.Id, ServiceFeeLevel.Level1);
                await jurisdictions.SetJurisdictionDescriptionAsync(added.Id, description);
                return;
            }

            // An operator may have moved a jurisdiction to a different service fee level;
            // that is a local decision, so only the description is refreshed from the corpus.
            await jurisdictions.SetJurisdictionDescriptionAsync(existing.Id, description);
        }

        private async Task UpsertFee(string code, string description, string source, IList<Guid> referencedModules)
        {
            var name = $"PCT-{code}";
            var existing = (await fees.GetFees()).FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            Guid id;

            if (existing is null)
            {
                var added = await fees.AddFeeAsync(name);
                if (!added.Success) throw new InvalidOperationException(added.Reason);
                id = added.Id;
            }
            else
            {
                id = existing.Id;
            }

            await fees.SetFeeDescriptionAsync(id, description);
            await fees.SetFeeJurisdictionNameAsync(id, code);
            await fees.SetFeeCategoryAsync(id, FeeCategory.OfficialFees);
            await fees.SetFeeSourceCodeAsync(id, source);
            await fees.SetReferencedModules(id, referencedModules);
        }
    }
}
