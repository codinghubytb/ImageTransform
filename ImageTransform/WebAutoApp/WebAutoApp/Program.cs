
using librarymongodb.Services;
using WebAutoApp.Client.Services;
using WebAutoApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebAutoApp.Client._Imports).Assembly);

app.Run();
