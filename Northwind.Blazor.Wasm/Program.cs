using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Northwind.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();