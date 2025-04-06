using Northwind.EntityModels; // To use AddNorthwindContext method.
using Microsoft.Extensions.Caching.Hybrid; // To use HybridCacheEntryOptions.
using Northwind.WebApi.Repositories; // To use ICustomerRepository.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast/{days:int?}",
    (int days = 5) => GetWeather(days))
    .WithName("GetWeatherForecast");

app.MapCustomers();

app.Run();
