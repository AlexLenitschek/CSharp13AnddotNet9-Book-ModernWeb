using Northwind.EntityModels; // To use AddNorthwindContext method.
using Microsoft.Extensions.Caching.Hybrid; // To use HybridCacheEntryOptions.
using Northwind.WebApi.Repositories; // To use ICustomerRepository.
using Microsoft.AspNetCore.HttpLogging; // To use HttpLoggingFields.
using System.Runtime.InteropServices; // To use RuntimeInformation

const string corsPolicyName = "allowWasmClient";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(documentName: "v2");
builder.Services.AddNorthwindContext();

// Add/Register HybridCache services
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromSeconds(60),
        LocalCacheExpiration = TimeSpan.FromSeconds(30)
    };
});

// Registers the CustomerRepository for use at runtime as a scoped dependency.
// !!! Can only use scoped dependencies inside other scoped dependencies !!!.
// Database context is also registered as a scoped dependency. Therefore cannot register repo as singleton.
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Add logging to the request pipeline.
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // 4 KB // Default is 32 KB.
    options.ResponseBodyLogLimit = 4096; // 4 KB // Default is 32 KB.
});

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Logging.AddEventLog();
}
// builder.Logging.AddEventLog(); Only supported by Windows and throws warning. Above removes warning.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins("https://localhost:5152", "https://localhost:5153");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.MapGet("/weatherforecast/{days:int?}",
    (int days = 5) => GetWeather(days))
    .WithName("GetWeatherForecast");

app.MapCustomers();

app.Run();
