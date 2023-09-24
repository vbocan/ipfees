using IPFees.API.Data;
using IPFees.API.Services;
using IPFees.Core.CurrencyConversion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;

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

// Configure settings
builder.Services.Configure<ServiceKeys>(builder.Configuration.GetSection(ServiceKeys.SectionName));
builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ServiceKeys>>().Value);

// Add exhange rate service
builder.Services.AddHostedService<ExchangeRateService>();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();
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
