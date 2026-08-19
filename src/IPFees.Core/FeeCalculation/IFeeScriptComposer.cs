using IPFees.Core.Model;
using IPFLang.Versioning;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Combines a fee script with the modules it depends on into a single executable script.
    ///
    /// Modules hold input declarations shared across many fees. IPFLang expresses that kind of
    /// reuse through jurisdiction composition, so each module becomes a parent of the next and
    /// the fee script is the leaf. A definition in the leaf overrides one of the same name
    /// inherited from a module.
    /// </summary>
    public interface IFeeScriptComposer
    {
        /// <summary>
        /// Compose a script from its modules.
        /// </summary>
        /// <param name="sourceCode">The fee script itself.</param>
        /// <param name="label">Identifier for the script in error messages, e.g. the fee name.</param>
        /// <param name="referencedModules">Modules the script explicitly depends on.</param>
        /// <param name="availableModules">All known modules; autorun members are always included.</param>
        /// <returns>The composed script, or null with the reasons composition failed.</returns>
        (ParsedScript? Script, IReadOnlyList<string> Errors) Compose(
            string sourceCode,
            string label,
            IEnumerable<Guid> referencedModules,
            IEnumerable<ModuleInfo> availableModules);
    }
}
