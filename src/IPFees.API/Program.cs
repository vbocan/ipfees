using Asp.Versioning;
using IPFees.API.Data;
using IPFees.API.Filters;
using IPFees.API.Services;
using IPFLang.Engine;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Repository;
using IPFLang.Parser;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using IPFLang.CurrencyConversion;

// Configure GuidRepresentation globally.
// This must match IPFees.Web, which writes the documents this process reads. Without it the
// driver refuses every document keyed by a Guid, failing any request that touches fees with
// "GuidSerializer cannot deserialize a Guid when GuidRepresentation is Unspecified".
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

// Set Serilog settings
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
    .MinimumLevel.Debug()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
                   ForwardedHeaders.XForwardedHost |     //Not included in the defaults using ASPNETCORE_FORWARDEDHEADERS_ENABLED
                   ForwardedHeaders.XForwardedFor |
                   ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 2;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add API versioning to the services.
builder.Services.AddApiVersioning(options =>
{
    // Specify the default API version (e.g., 1.0).
    options.DefaultApiVersion = new ApiVersion(1, 0);
    // Include the API version in the response headers.
    options.ReportApiVersions = true;
    // Specify the supported API versions.
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"));
});

builder.Services.AddSwaggerGen(c =>
{
    //Following code to avoid swagger generation error due to same method name in different versions.
    c.ResolveConflictingActions(descriptions =>
    {
        return descriptions.First();
    });

    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "IPFees API",
        Version = "1",
        Description =
            "Computes intellectual property fees for supported jurisdictions and currencies.\n\n" +
            "**This API is public and requires no authentication.** It serves the same fee " +
            "schedules as the web calculator, all of which are published by the patent offices " +
            "themselves. There is no rate limiting, so treat it as a reference service rather " +
            "than a dependency for production workloads.\n\n" +
            "Note that `/Fee/Verify` runs static analysis over a fee schedule and is far more " +
            "expensive than the other endpoints; a schedule declaring a monotonicity directive " +
            "will consume the full verification budget before responding.",
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Name = "Valer Bocan, PhD, CSSLP",
            Email = "valer@bocan.ro",
        },
    });
    c.OperationFilter<RemoveVersionFromParameter>();
    c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
});

// Configure settings
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ConnectionStrings>>().Value);
builder.Services.Configure<ServiceKeys>(builder.Configuration.GetSection(ServiceKeys.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ServiceKeys>>().Value);

// Add exhange rate service
builder.Services.AddHostedService<ExchangeRateService>();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddTransient<IExchangeRateFetcher>(x => new ExchangeRateFetcher(x.GetRequiredService<IOptions<ServiceKeys>>().Value.ExchangeRateApiKey));

// Register work database context (MongoDB)
// The MongoDB client has a pool of connections that are reused automatically and a single MongoDB client instance is enough even in multithreaded scenarios
// See http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/ (Mongo Client section)
builder.Services.AddSingleton<DataContext>(x => new DataContext(x.GetRequiredService<IOptions<ConnectionStrings>>().Value.MongoDbConnection));

// Add services
builder.Services.AddTransient<IDslParser, DslParser>();
builder.Services.AddTransient<IDslCalculator, DslCalculator>();
builder.Services.AddTransient<IFeeRepository, FeeRepository>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IJurisdictionRepository, JurisdictionRepository>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IFeeScriptComposer, FeeScriptComposer>();
builder.Services.AddTransient<ICorpusSeeder, CorpusSeeder>();
builder.Services.AddSingleton(new VerificationBudget());
builder.Services.AddTransient<IFeeDefinitionValidator, FeeDefinitionValidator>();
builder.Services.AddTransient<IFeeCalculator, FeeCalculator>();
builder.Services.AddTransient<IJurisdictionFeeManager, JurisdictionFeeManager>();
builder.Services.AddTransient<IExchangeRateFetcher>(x => new ExchangeRateFetcher(x.GetRequiredService<IOptions<ServiceKeys>>().Value.ExchangeRateApiKey));

// Add logger
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
// TODO: Disable Swagger UI in production!
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
// }
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
