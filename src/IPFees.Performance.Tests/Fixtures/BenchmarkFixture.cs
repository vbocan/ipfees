using DotNet.Testcontainers.Containers;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Testcontainers.MongoDb;

namespace IPFees.Performance.Tests.Fixtures
{
    /// <summary>
    /// Shared fixture for benchmarks that require MongoDB
    /// </summary>
    public class BenchmarkFixture : IAsyncDisposable
    {
        public FeeRepository FeeRepository { get; set; }
        public JurisdictionRepository JurisdictionRepository { get; set; }
        public ModuleRepository ModuleRepository { get; set; }
        public SettingsRepository SettingsRepository { get; set; }
        public DataContext DbContext { get; private set; }
        
        private readonly IContainer mongoContainer;
        private readonly string connectionString = string.Empty;
        private bool isInitialized = false;

        public BenchmarkFixture()
        {
            try
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            catch
            {
                // Ignore if already registered
            }

            // Start MongoDB container
            mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:8.0")
                .WithPortBinding(27017, true)
                .WithEnvironment("MONGO_INITDB_DATABASE", "IPFeesBenchmark")
                .Build();

            mongoContainer.StartAsync().GetAwaiter().GetResult();

            connectionString = $"mongodb://localhost:{mongoContainer.GetMappedPublicPort(27017)}/IPFeesBenchmark";
            DbContext = new DataContext(connectionString);

            FeeRepository = new FeeRepository(DbContext);
            JurisdictionRepository = new JurisdictionRepository(DbContext);
            ModuleRepository = new ModuleRepository(DbContext);
            SettingsRepository = new SettingsRepository(DbContext);
        }

        public async Task InitializeTestDataAsync()
        {
            if (isInitialized) return;

            // Create sample modules
            await CreateSampleModulesAsync();
            
            // Create sample jurisdictions
            await CreateSampleJurisdictionsAsync();
            
            isInitialized = true;
        }

        private async Task CreateSampleModulesAsync()
        {
            // Module 1: Simple fee calculation
            var mod1 = await ModuleRepository.AddModuleAsync("BasicModule");
            if (mod1.Success)
            {
                await ModuleRepository.SetModuleSourceCodeAsync(mod1.Id, 
                    """
                    COMPUTE FEE BasicModuleFee
                    YIELD 100
                    ENDCOMPUTE
                    """);
            }

            // Module 2: Conditional fee calculation
            var mod2 = await ModuleRepository.AddModuleAsync("ConditionalModule");
            if (mod2.Success)
            {
                await ModuleRepository.SetModuleSourceCodeAsync(mod2.Id,
                    """
                    DEFINE LIST EntitySize AS 'Entity size'
                    CHOICE Entity_Company AS 'Company'
                    CHOICE Entity_Person AS 'Individual'
                    DEFAULT Entity_Company
                    ENDDEFINE
                    
                    COMPUTE FEE ConditionalFee
                    LET CompanyFee AS 500
                    LET PersonFee AS 250
                    YIELD CompanyFee IF EntitySize EQ Entity_Company
                    YIELD PersonFee IF EntitySize EQ Entity_Person
                    ENDCOMPUTE
                    """);
            }
        }

        private async Task CreateSampleJurisdictionsAsync()
        {
            // Simple jurisdiction
            await CreateSimpleJurisdiction();
            
            // Complex jurisdiction (like EPO)
            await CreateComplexJurisdiction();
            
            // Medium complexity jurisdiction
            await CreateMediumJurisdiction();
        }

        private async Task CreateSimpleJurisdiction()
        {
            var jur = await JurisdictionRepository.AddJurisdictionAsync("TEST_SIMPLE");
            if (!jur.Success) return;

            await JurisdictionRepository.SetJurisdictionDescriptionAsync(jur.Id, "Simple Test Jurisdiction");

            var fee = await FeeRepository.AddFeeAsync("TEST_SIMPLE");
            if (!fee.Success) return;

            await FeeRepository.SetFeeSourceCodeAsync(fee.Id,
                """
                RETURN Currency AS 'USD'
                
                DEFINE NUMBER SheetCount AS 'Number of sheets'
                BETWEEN 1 AND 1000
                DEFAULT 30
                ENDDEFINE
                
                COMPUTE FEE BasicFee
                YIELD 100
                ENDCOMPUTE
                
                COMPUTE FEE SheetFee
                LET Fee AS 5
                YIELD Fee * (SheetCount - 30) IF SheetCount GT 30
                ENDCOMPUTE
                """);

            // Note: In actual implementation, fees are linked via the jurisdiction's FeeId property
            // We'll rely on name matching for test purposes
        }

        private async Task CreateMediumJurisdiction()
        {
            var jur = await JurisdictionRepository.AddJurisdictionAsync("TEST_MEDIUM");
            if (!jur.Success) return;

            await JurisdictionRepository.SetJurisdictionDescriptionAsync(jur.Id, "Medium Test Jurisdiction");

            var fee = await FeeRepository.AddFeeAsync("TEST_MEDIUM");
            if (!fee.Success) return;

            await FeeRepository.SetFeeSourceCodeAsync(fee.Id,
                """
                RETURN Currency AS 'EUR'
                
                DEFINE LIST EntitySize AS 'Entity size'
                CHOICE Entity_Company AS 'Company'
                CHOICE Entity_Person AS 'Individual'
                DEFAULT Entity_Company
                ENDDEFINE
                
                DEFINE NUMBER SheetCount AS 'Number of sheets'
                BETWEEN 1 AND 1000
                DEFAULT 30
                ENDDEFINE
                
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 100
                DEFAULT 10
                ENDDEFINE
                
                COMPUTE FEE BasicNationalFee
                LET Fee1 AS 400
                LET Fee2 AS 200
                YIELD Fee1 IF EntitySize EQ Entity_Company
                YIELD Fee2 IF EntitySize EQ Entity_Person
                ENDCOMPUTE
                
                COMPUTE FEE SheetFee
                LET Fee AS 10
                YIELD Fee * (SheetCount - 30) IF SheetCount GT 30
                ENDCOMPUTE
                
                COMPUTE FEE ClaimFee
                LET Fee AS 50
                YIELD Fee * (ClaimCount - 20) IF ClaimCount GT 20
                ENDCOMPUTE
                """);
        }

        private async Task CreateComplexJurisdiction()
        {
            var jur = await JurisdictionRepository.AddJurisdictionAsync("TEST_COMPLEX");
            if (!jur.Success) return;

            await JurisdictionRepository.SetJurisdictionDescriptionAsync(jur.Id, "Complex Test Jurisdiction");

            var fee = await FeeRepository.AddFeeAsync("TEST_COMPLEX");
            if (!fee.Success) return;

            await FeeRepository.SetFeeSourceCodeAsync(fee.Id,
                """
                RETURN Currency AS 'EUR'
                
                DEFINE LIST ISA AS 'International Search Authority'
                CHOICE ISA_EPO AS 'EPO'
                CHOICE ISA_USPTO AS 'USPTO'
                CHOICE ISA_JPO AS 'JPO'
                CHOICE ISA_OTHER AS 'Other'
                DEFAULT ISA_EPO
                ENDDEFINE
                
                DEFINE LIST IPRP AS 'International Preliminary Report on Patentability'
                CHOICE IPRP_EPO AS 'EPO'
                CHOICE IPRP_NONE AS 'None'
                DEFAULT IPRP_NONE
                ENDDEFINE
                
                DEFINE NUMBER SheetCount AS 'Number of sheets'
                BETWEEN 1 AND 1000
                DEFAULT 35
                ENDDEFINE
                
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 100
                DEFAULT 15
                ENDDEFINE
                
                DEFINE BOOLEAN Examination AS 'Request examination'
                DEFAULT true
                ENDDEFINE
                
                COMPUTE FEE BasicNationalFee
                YIELD 135
                ENDCOMPUTE
                
                COMPUTE FEE DesignationFee
                YIELD 660
                ENDCOMPUTE
                
                COMPUTE FEE SheetFee
                LET Fee AS 17
                YIELD Fee * (SheetCount - 35) IF SheetCount GT 35
                ENDCOMPUTE
                
                COMPUTE FEE ClaimFee
                LET StandardFee AS 265
                LET OverLimitFee AS 665
                LET ExcessClaims AS ClaimCount - 15
                LET Over50 AS ClaimCount - 50
                
                YIELD (StandardFee * ExcessClaims) IF (ClaimCount GT 15) AND (ClaimCount LE 50)
                YIELD (StandardFee * 35) + (OverLimitFee * Over50) IF ClaimCount GT 50
                ENDCOMPUTE
                
                COMPUTE FEE SearchFee
                LET FullFee AS 1430
                LET ReducedFee AS 143
                
                YIELD 0 IF ISA EQ ISA_EPO
                YIELD ReducedFee IF (ISA EQ ISA_USPTO) OR (ISA EQ ISA_JPO)
                YIELD FullFee IF ISA EQ ISA_OTHER
                ENDCOMPUTE
                
                COMPUTE FEE ExaminationFee
                LET Fee AS 2055
                YIELD Fee IF Examination EQ true
                ENDCOMPUTE
                """);
        }

        public async ValueTask DisposeAsync()
        {
            await mongoContainer.DisposeAsync();
        }
    }
}
