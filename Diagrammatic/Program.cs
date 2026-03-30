using Diagrammatic_test.Components;
using Diagrammatic_test.Services;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;

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

// 1. Register your custom service
builder.Services.AddScoped<AuthStateProviderService>();

// 2. Map it to the standard Blazor provider
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<AuthStateProviderService>());

// 3. Use AuthorizationCore (this is for Blazor logic)
builder.Services.AddAuthorizationCore();

// ------------------------
// Third-party services
// ------------------------
builder.Services.AddBlazoredSessionStorage();
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

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
