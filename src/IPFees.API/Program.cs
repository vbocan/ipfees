using Asp.Versioning;
using IPFees.API.Data;
using IPFees.API.Filters;
using IPFees.API.Services;
using IPFees.API.Validator;
using IPFees.Calculator;
using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Repository;
using IPFees.Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

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
    options.KnownNetworks.Clear();
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

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IPFees API",
        Version = "1",
        Description = "A simple API to compute the IP fees for supported jurisdictions and currencies.",
        Contact = new OpenApiContact
        {
            Name = "Valer Bocan, PhD, CSSLP",
            Email = "valer.bocan@storya.ro",
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
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

// Add logger
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Add authentication
var KeycloakSection = builder.Configuration.GetSection(KeycloakSettings.SectionName);
var KeycloakServer = KeycloakSection.GetValue<string>("Server");
var KeycloakRealm = KeycloakSection.GetValue<string>("Realm");
var KeycloakClientID = KeycloakSection.GetValue<string>("ClientID");
var KeycloakClientSecret = KeycloakSection.GetValue<string>("ClientSecret");
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = $"{KeycloakServer}/auth/realms/{KeycloakRealm}";
    options.ClientId = $"{KeycloakClientID}";
    options.ClientSecret = $"{KeycloakClientSecret}";
    options.MetadataAddress = $"{KeycloakServer}/realms/{KeycloakRealm}/.well-known/openid-configuration";
    options.RequireHttpsMetadata = false;
    options.SaveTokens = true;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles"); // Request Keycloak roles
    options.ClaimActions.MapUniqueJsonKey("role", "role"); // Map the "role" claim to the role in the token        
    options.NonceCookie.SameSite = SameSiteMode.Unspecified;
    options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = $"{KeycloakClientID}",
        ValidateIssuer = true,
        //NameClaimType = "name",
        //RoleClaimType = ClaimTypes.Role,
        ValidIssuer = $"{KeycloakServer}/auth/realms/{KeycloakRealm}",
    };
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            // Inspect the claims in the token
            var claims = context.Principal.Claims.ToList();
            // Log or debug print the claims to ensure the "role" claim is present

            // You can customize role handling here if needed.
            return Task.CompletedTask;
        }
    };
});

// Add authorization
builder.Services.AddAuthorization();

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
