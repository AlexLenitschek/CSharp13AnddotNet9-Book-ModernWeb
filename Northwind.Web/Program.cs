using Northwind.Web.Components; // To use App.
using Northwind.EntityModels; // To use AddNorthwindContext method.

#region Configure the web server host and services.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddNorthwindContext();

var app = builder.Build();

#endregion

#region Configure the HTTP pipeline and routes

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

#region Implementing an anonymous inline delegate as middleware
// Implementing an anonymous inline delegate as middleware
// to intercept HTTP requests and responses.
app.Use(async (HttpContext context, Func<Task> next) =>
{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if (rep is not null)
    {
        WriteLine($"Endpoint name: {rep.DisplayName}");
        WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
    }

    if (context.Request.Path == "/bonjour")
    {
        // In the case of a match on URL path, this becomes a terminating
        // delegate that returns so does not call the next delegate.
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }

    // We could modify the request before calling the next delegate.
    await next();

    // We could modify the response after calling the next delegate.
});
#endregion

app.UseHttpsRedirection(); // Redirect from Http to Https with 308 response.

app.UseAntiforgery();

app.UseDefaultFiles(); // enables default file mapping on the current path to identify default files
                       // like index.html, default.html, and so on. // THIS CALL MUST BE BEFORE THE NEXT TWO!
//app.MapStaticAssets(); // .NET9 or later.
app.UseStaticFiles(); // .NET8 or earlier. Adds middleware that looks in wwwroot for static files to return in the HTTP response.

//app.MapRazorPages(); // Adds middleware that will map URL paths such as `/suppliers`
                       // to a Razor Page file in the `/Pages` folder named `suppliers.cshtml`
                       // and return the results as the HTTP response.
app.MapRazorComponents<App>(); // Same but for blazor components instead of razor pages.


// MapGet(): Adds middleware that will map URL paths such as `/hello` to an inline delegate
// that writes plain text directly to the HTTP response.
app.MapGet("/env", () => $"Environment is {app.Environment.EnvironmentName}");

app.MapGet("/data", () => Results.Json(new
{
    firstName = "John",
    lastName = "Doe",
    age = 30
}));

app.MapGet("/welcome", () => Results.Content(
    content: $"""
    <!doctype html>
    <html lang = "en">
    <head>
        <title>Welcome to Northwind Web!</title>
    </head>
    <body>
        <h1>Welcome to Northwind Web!</h1>
    </body>
    </html>
    """, 
    contentType: "text/html"));

#endregion

// Start the web server, host the website, and wait for requests.
app.Run(); // This is a thread-blocking call.
WriteLine("This executes after the web server has stopped!");
