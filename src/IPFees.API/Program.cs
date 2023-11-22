using Asp.Versioning;
using IPFees.API.Data;
using IPFees.API.Filters;
using IPFees.API.Services;
using IPFees.Calculator;
using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Repository;
using IPFees.Parser;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IPFees API",
        Version = "1"
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
builder.Services.AddTransient<IExchangeRateFetcher>(x => new ExchangeRateFetcher(x.GetService<IOptions<ServiceKeys>>().Value.ExchangeRateApiKey));

// Register work database context (MongoDB)
// The MongoDB client has a pool of connections that are reused automatically and a single MongoDB client instance is enough even in multithreaded scenarios
// See http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/ (Mongo Client section)
builder.Services.AddSingleton<DataContext>(x => new DataContext(x.GetService<IOptions<ConnectionStrings>>().Value.MongoDbConnection));

// Add services
builder.Services.AddTransient<IDslParser, DslParser>();
builder.Services.AddTransient<IDslCalculator, DslCalculator>();
builder.Services.AddTransient<IFeeRepository, FeeRepository>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IJurisdictionRepository, JurisdictionRepository>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IFeeCalculator, FeeCalculator>();
builder.Services.AddTransient<IJurisdictionFeeManager, JurisdictionFeeManager>();
builder.Services.AddTransient<IExchangeRateFetcher>(x => new ExchangeRateFetcher(x.GetService<IOptions<ServiceKeys>>().Value.ExchangeRateApiKey));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
