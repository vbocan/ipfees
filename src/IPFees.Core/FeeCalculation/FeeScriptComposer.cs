using IPFees.Core.Model;
using IPFLang.Composition;
using IPFLang.Parser;
using IPFLang.Versioning;

namespace IPFees.Core.FeeCalculation
{
    /// <inheritdoc cref="IFeeScriptComposer"/>
    public class FeeScriptComposer : IFeeScriptComposer
    {
        private readonly IDslParser Parser;

        public FeeScriptComposer(IDslParser parser)
        {
            Parser = parser;
        }

        public (ParsedScript? Script, IReadOnlyList<string> Errors) Compose(
            string sourceCode,
            string label,
            IEnumerable<Guid> referencedModules,
            IEnumerable<ModuleInfo> availableModules)
        {
            var modules = availableModules.ToList();
            var registry = new JurisdictionRegistry();
            string? parentId = null;
            string? leafId = null;

            foreach (var (id, source) in BuildChain(sourceCode, label, referencedModules, modules))
            {
                var (parsed, errors) = Parser.Parse(source, returnParsedScript: true);
                if (parsed is null)
                {
                    var detail = errors.Any()
                        ? errors.Select(e => $"{id}: {e}").ToList()
                        : new List<string> { $"{id}: could not be parsed." };
                    return (null, detail);
                }

                registry.Register(new Jurisdiction(id, id, parsed, parentId));
                parentId = id;
                leafId = id;
            }

            try
            {
                var composed = new JurisdictionComposer(registry).Compose(leafId!);
                return (composed.Script, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                return (null, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Order the sources that make up a script: autorun modules first, then the modules the
        /// script explicitly references, then the script itself. Each entry inherits from the one
        /// before it, so the script has the final say on any name it redefines.
        /// </summary>
        private static IEnumerable<(string Id, string SourceCode)> BuildChain(
            string sourceCode,
            string label,
            IEnumerable<Guid> referencedModules,
            IReadOnlyCollection<ModuleInfo> availableModules)
        {
            var seen = new HashSet<Guid>();

            foreach (var autorun in availableModules.Where(w => w.AutoRun))
            {
                if (seen.Add(autorun.Id)) yield return ($"module:{autorun.Name}", autorun.SourceCode);
            }

            foreach (var id in referencedModules)
            {
                var module = availableModules.SingleOrDefault(w => w.Id.Equals(id))
                    ?? throw new NotSupportedException($"Module '{id}' does not exist.");
                if (seen.Add(module.Id)) yield return ($"module:{module.Name}", module.SourceCode);
            }

            yield return (label, sourceCode);
        }
    }
}
