using IPFees.Calculator;
using IPFees.Evaluator;
using IPFFees.Core.Data;
using IPFFees.Core.Models;
using Mapster;
using MongoDB.Driver;
using System.Diagnostics.Tracing;
using System.Runtime.Versioning;
using ZstdSharp;
using static System.Net.Mime.MediaTypeNames;

namespace IPFFees.Core
{
    public class Jurisdiction : IJurisdiction
    {
        private readonly DataContext context;
        private readonly IModule module;
        private readonly IIPFCalculator calculator;

        public Jurisdiction(DataContext context, IModule module, IIPFCalculator calculator)
        {
            this.context = context;
            this.module = module;
            this.calculator = calculator;
        }
        /// <summary>
        /// Create a new jurisdiction
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name.</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        /// <remarks>
        /// A jurisdiction is an area where Intellectual Property rules are in effect and usually consists of one or more countries.        
        /// </remarks>
        public async Task<DbResult> AddJurisdictionAsync(string JurisdictionName)
        {
            if (GetJurisdictions().Any(a => a.Name.Equals(JurisdictionName)))
            {
                return DbResult.Fail($"A jurisdiction named '{JurisdictionName}' already exists.");
            }
            var newDoc = new JurisdictionDoc
            {
                Name = JurisdictionName,
                LastUpdatedOn = DateTime.UtcNow.ToLocalTime()
            };
            try
            {
                await context.JurisdictionCollection.InsertOneAsync(newDoc);
                return DbResult.Succeed(newDoc.Id);
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }
        /// <summary>
        /// Set the jurisdiction description
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <param name="Description">Description of the functionality provided</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionDescriptionAsync(string JurisdictionName, string Description)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Name.Equals(JurisdictionName),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.Description, Description));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the jurisdiction source code
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <param name="SourceCode">Source code of the jurisdiction</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionSourceCodeAsync(string JurisdictionName, string SourceCode)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Name.Equals(JurisdictionName),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.SourceCode, SourceCode));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get all registered jurisdictions.
        /// </summary>
        /// <returns>An enumeration of JurisdictionInfo objects</returns>
        public IEnumerable<JurisdictionInfo> GetJurisdictions()
        {
            var dbObjs = context.JurisdictionCollection.AsQueryable();
            return dbObjs.Adapt<IEnumerable<JurisdictionInfo>>();
        }

        /// <summary>
        /// Get jurisdiction by name
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <returns>A JurisdictionInfo object</returns>
        public JurisdictionInfo GetJurisdictionByName(string JurisdictionName)
        {
            var dbObjs = context.JurisdictionCollection.AsQueryable().Where(w => w.Name.Equals(JurisdictionName)).Single();
            return dbObjs.Adapt<JurisdictionInfo>();
        }

        /// <summary>
        /// Remove a specified jurisdiction
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> RemoveJurisdictionAsync(string JurisdictionName)
        {
            try
            {
                await context.JurisdictionCollection.DeleteOneAsync(s => s.Name.Equals(JurisdictionName));
                return DbResult.Succeed();
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }

        /// <summary>
        /// Set the jurisdiction description
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <param name="ModuleNames">An array of module names referenced by the jurisdiction</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetReferencedModules(string JurisdictionName, string[] ModuleNames)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Name.Equals(JurisdictionName),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.ReferencedModules, ModuleNames));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Compute fees related to the given jurisdiction
        /// </summary>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <param name="Vars">Calculation parameters</param>
        public void ComputeFees(string JurisdictionName, IList<IPFValue> Vars)
        {
            var jur = GetJurisdictionByName(JurisdictionName) ?? throw new NotSupportedException($"Jurisdiction '{JurisdictionName}' does not exist.");
            // Step 1: Parse the source code of the referenced modules (if any)
            foreach (var rm in jur.ReferencedModules)
            {
                // Retrieve the referenced module
                var mod = module.GetModuleByName(rm) ?? throw new NotSupportedException($"Module '{rm}' does not exist.");
                calculator.Parse(mod.SourceCode);
            }
            // Step 2: Parse the source code of the current jurisdiction
            calculator.Parse(jur.SourceCode);

            // Step 3: Process calculation results
            var CalcErrors = calculator.GetErrors();

            //List<IPFValue> vars = new() {
            //    new IPFValueString("EntityType", "NormalEntity"),
            //    new IPFValueString("SituationType", "PreparedISA"),
            //    new IPFValueNumber("SheetCount", 120),
            //    new IPFValueNumber("ClaimCount", 7)
            //};

            var (TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps) = calculator.Compute(Vars);

        }
    }
}