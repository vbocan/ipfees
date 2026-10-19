using IPFLang.Engine;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Repository;
using IPFLang.Parser;
using IPFees.Web.Data;
using IPFees.Web.Services;
using IPFees.Web.Authoring;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using IPFLang.CurrencyConversion;

// Configure GuidRepresentation globally
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

// Set Serilog settings
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
    .MinimumLevel.Debug()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
// Calculating is open to anyone; changing what the instance serves requires signing in with the
// authoring password (Authoring:Password). Without one the instance is read-only.
builder.Services.Configure<AuthoringSettings>(builder.Configuration.GetSection(AuthoringSettings.SectionName));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "IPFees.Author";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddRazorPages(options =>
{
    // Pages that exist only to change what the instance serves open only after signing in. The
    // edit pages of fee definitions and modules stay readable, because a calculation links to the
    // IPFLang source behind it; AuthoringPostFilter refuses their forms to anonymous visitors.
    options.Conventions.AuthorizeAreaPage("Fee", "/Create");
    options.Conventions.AuthorizeAreaPage("Fee", "/Delete");
    options.Conventions.AuthorizeAreaPage("Module", "/Create");
    options.Conventions.AuthorizeAreaPage("Module", "/Delete");
    options.Conventions.AuthorizeAreaPage("Jurisdiction", "/Create");
    options.Conventions.AuthorizeAreaPage("Jurisdiction", "/Edit");
    options.Conventions.AuthorizeAreaPage("Jurisdiction", "/Delete");
    options.Conventions.AuthorizeAreaPage("Settings", "/EditServiceFee");
}).AddMvcOptions(options => options.Filters.Add(new AuthoringPostFilter()));

// Configure settings
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ConnectionStrings>>().Value);
builder.Services.Configure<ServiceKeys>(builder.Configuration.GetSection(ServiceKeys.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ServiceKeys>>().Value);
builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection(CurrencySettings.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<CurrencySettings>>().Value);

// Register work database context (MongoDB)
// The MongoDB client has a pool of connections that are reused automatically and a single MongoDB client instance is enough even in multithreaded scenarios
// See http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/ (Mongo Client section)
builder.Services.AddSingleton<DataContext>(x => new DataContext(x.GetRequiredService<IOptions<ConnectionStrings>>().Value.MongoDbConnection));

// Add IPFees services
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

builder.Services.AddHttpContextAccessor();

// Add exhange rate service
builder.Services.AddHostedService<ExchangeRateService>();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();

// Add database reset service
builder.Services.AddHostedService<DatabaseResetService>();
builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));
// Replace the line that registers IMongoClient with the following:
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<IOptions<ConnectionStrings>>().Value.MongoDbConnection));

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
