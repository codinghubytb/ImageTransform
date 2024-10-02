using librarymongodb.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Features;
using WebApp;
using WebApp.Components;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Access the Logger configuration from appsettings.json
var logDirectory = builder.Configuration["Logger:LogDirectory"];
var logFileName = string.Format(builder.Configuration["Logger:LogFileName"], DateTime.Now.ToString("yyyy-MM-dd"));

// Ensure the log directory exists
Directory.CreateDirectory(logDirectory); // Creates the directory if it doesn't exist

// Combine the log directory and filename
string logFilePath = Path.Combine(logDirectory, logFileName);

// Register the logger as a singleton service
builder.Services.AddSingleton(new LibraryLogs.Logger(logFilePath));



// Configure HttpClient with base address
#if DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["Connection:Debug"].ToString()) });
#else
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["Connection:Release"].ToString()) });
#endif

// Register WebService
builder.Services.AddScoped(sp =>
{
    string database = builder.Configuration["Connection:Database"].ToString();
    HttpClient httpClient = new HttpClient { BaseAddress = new Uri(builder.Configuration["Connection:Debug"].ToString()) };
    return new WebService(database, httpClient);
});

// Register ModuleService
builder.Services.AddScoped(sp =>
{
    HttpClient httpClient = new HttpClient { BaseAddress = new Uri(builder.Configuration["Connection:DebugApiModule"].ToString()) };
    return new ModuleService(httpClient);
});
builder.Services.AddSingleton<GUI_APP>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
