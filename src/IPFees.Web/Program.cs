using IPFees.Calculator;
using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Repository;
using IPFees.Parser;
using IPFees.Web.Data;
using IPFees.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Formatting.Compact;

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
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://www.jet-ip.legal");
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});


// Add services to the container.
builder.Services.AddRazorPages();

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
builder.Services.AddSingleton<DataContext>(x => new DataContext(x.GetService<IOptions<ConnectionStrings>>().Value.MongoDbConnection));

// Add IPFees services
builder.Services.AddTransient<IDslParser, DslParser>();
builder.Services.AddTransient<IDslCalculator, DslCalculator>();
builder.Services.AddTransient<IFeeRepository, FeeRepository>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IJurisdictionRepository, JurisdictionRepository>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IFeeCalculator, FeeCalculator>();
builder.Services.AddTransient<IJurisdictionFeeManager, JurisdictionFeeManager>();
builder.Services.AddTransient<IExchangeRateFetcher>(x => new ExchangeRateFetcher(x.GetService<IOptions<ServiceKeys>>().Value.ExchangeRateApiKey));

builder.Services.AddHttpContextAccessor();

// Add exhange rate service
builder.Services.AddHostedService<ExchangeRateService>();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
