using Diagrammatic_test.Components;
using Diagrammatic_test.Services;
using Blazored.Toast;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// Add services to DI
// ------------------------

// Add Razor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HTTP client for your API
builder.Services.AddHttpClient("DiagrammaticClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:8083/gameHub");
});

// Refresh service (singleton + hosted service)
builder.Services.AddSingleton<RefreshService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<RefreshService>());

// ------------------------
// Authentication / Authorization
// ------------------------

// Register AuthStateProviderService as concrete type
builder.Services.AddScoped<AuthStateProviderService>();

// Register it as the AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProviderService>();

// Add authentication with default scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";        // redirect unauthorized users
        options.AccessDeniedPath = "/error"; // optional
    });

// Add authorization core for Blazor
builder.Services.AddAuthorizationCore();

// ------------------------
// Third-party services
// ------------------------
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazorBootstrap();

// Razor Pages (required for Blazor Server)
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// ------------------------
// Build the app
// ------------------------
var app = builder.Build();

// ------------------------
// Configure HTTP request pipeline
// ------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
