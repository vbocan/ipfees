using System.Security.Cryptography;
using System.Text;
using IPFees.Core.FeeCalculation;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Core.Data;
using IPFLang.Corpus;
using IPFLang.Versioning;
using N = IPFLang.Parser;
using NE = IPFLang.Engine;
using NV = IPFLang.Evaluator;

namespace Harness.Common;

/// <summary>
/// The jurisdiction corpus packaged in IPFLang.Engine, as an IPFees deployment holds it after
/// CorpusSeeder has run: the four regional bases stored as (non-autorun) modules, and one fee
/// document per jurisdiction whose source is the corpus script and whose referenced module is
/// its base.
/// </summary>
public static class PackagedCorpus
{
    public static IReadOnlyList<string> Codes => JurisdictionCorpus.Codes;

    public static IReadOnlyList<ModuleInfo> Modules { get; } = JurisdictionCorpus.BaseNames
        .Select(name => new ModuleInfo(IdFor(name), name, $"IPFLang regional base '{name}'", JurisdictionCorpus.GetBase(name), false, DateTime.UnixEpoch))
        .ToList();

    public static string Source(string code) => JurisdictionCorpus.GetJurisdiction(code);

    public static IReadOnlyList<Guid> ReferencedModules(string code)
    {
        var baseName = JurisdictionCorpus.GetBaseNameFor(code);
        return baseName is null ? Array.Empty<Guid>() : new[] { IdFor(baseName) };
    }

    public static string? BaseName(string code) => JurisdictionCorpus.GetBaseNameFor(code);

    /// <summary>Compose through the platform's FeeScriptComposer, the path FeeCalculator uses.</summary>
    public static (ParsedScript? Script, IReadOnlyList<string> Errors) Compose(string code, string? sourceOverride = null) =>
        new FeeScriptComposer(new N.DslParser()).Compose(sourceOverride ?? Source(code), $"fee:{code}", ReferencedModules(code), Modules);

    public static FeeDefinitionValidator Validator(TimeSpan budget) =>
        new(new InMemoryModules(Modules), new FeeScriptComposer(new N.DslParser()), new NE.DslCalculator(new N.DslParser()),
            new VerificationBudget { Limit = budget });

    private static Guid IdFor(string name) =>
        new(MD5.HashData(Encoding.UTF8.GetBytes("ipflang-base:" + name)));

    /// <summary>Read-only module store standing in for MongoDB.</summary>
    private sealed class InMemoryModules : IModuleRepository
    {
        private readonly IReadOnlyList<ModuleInfo> modules;
        public InMemoryModules(IReadOnlyList<ModuleInfo> modules) => this.modules = modules;
        public Task<IEnumerable<ModuleInfo>> GetModules() => Task.FromResult<IEnumerable<ModuleInfo>>(modules);
        public Task<ModuleInfo> GetModuleById(Guid Id) => Task.FromResult(modules.Single(m => m.Id == Id));
        public Task<DbResult> AddModuleAsync(string Name) => throw new NotSupportedException();
        public Task<DbResult> SetModuleNameAsync(Guid Id, string Name) => throw new NotSupportedException();
        public Task<DbResult> SetModuleDescriptionAsync(Guid Id, string Description) => throw new NotSupportedException();
        public Task<DbResult> SetModuleSourceCodeAsync(Guid Id, string SourceCode) => throw new NotSupportedException();
        public Task<DbResult> SetModuleAutoRunStatusAsync(Guid Id, bool AutoRun) => throw new NotSupportedException();
        public Task<DbResult> RemoveModuleAsync(Guid Id) => throw new NotSupportedException();
    }
}

public static class FeeEval
{
    /// <summary>A calculator loaded with one fee of a composed script and all of its inputs.</summary>
    public static NE.DslCalculator CalculatorFor(ParsedScript script, N.DslFee fee)
    {
        var calculator = new NE.DslCalculator(new N.DslParser());
        calculator.LoadParsedScript(new ParsedScript(script.Inputs, new[] { fee }, script.Returns, script.Groups, Array.Empty<N.DslVerify>()));
        return calculator;
    }

    public static (decimal? Amount, string? Error) Fee(NE.DslCalculator calculator, IEnumerable<NV.IPFValue> values)
    {
        try
        {
            var (mandatory, optional, _, _) = calculator.Compute(values);
            return (mandatory + optional, null);
        }
        catch (Exception ex)
        {
            return (null, ex.GetType().Name + ": " + ex.Message);
        }
    }

    public static string? Currency(ParsedScript script) =>
        script.Returns.FirstOrDefault(r => r.Symbol.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))?.Text;
}
