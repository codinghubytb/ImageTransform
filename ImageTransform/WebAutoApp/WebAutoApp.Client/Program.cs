using librarymongodb.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAutoApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


// Charger la configuration depuis appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Enregistrer WebService
builder.Services.AddScoped(sp =>
{
    var apiSettings = builder.Configuration.GetSection("ApiSettings");
    HttpClient httpClient = new HttpClient { BaseAddress = new Uri(apiSettings["BaseUrl"]) };
    return new WebService(apiSettings["Database"], httpClient);
});

builder.Services.AddScoped(sp =>
{
    var apiSettings = builder.Configuration.GetSection("ApiSettings");
    HttpClient httpClient = new HttpClient { BaseAddress = new Uri(apiSettings["BaseUrlModule"]) };
    return new ModuleService(apiSettings["Database"], httpClient);

});


await builder.Build().RunAsync();
