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

// Where the API lives. Bound from the "Api" section so the address can differ per
// environment: over the compose network in Docker, on a published port locally.
builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.SectionName));

var apiOptions = builder.Configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>()
                 ?? new ApiOptions();

// HTTP client for your API. Every call site passes an absolute path ("/api/..."), so the
// base address only has to carry the scheme, host and port.
builder.Services.AddHttpClient("DiagrammaticClient", client =>
{
    client.BaseAddress = new Uri(apiOptions.InternalBaseUrl);
});

// Refresh service. Scoped, so each signed-in user's circuit refreshes its own token:
// as a singleton it held one token pair for the entire server, so concurrent users
// overwrote each other's and a single logout stopped refresh for everybody.
builder.Services.AddScoped<RefreshService>();

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

// Only redirect when this app is the one terminating TLS. In Docker it listens on plain
// HTTP and TLS belongs to whatever proxy sits in front, where this middleware has no HTTPS
// port to redirect to and can only log "Failed to determine the https port" on every request.
var httpsConfigured = (builder.Configuration["ASPNETCORE_URLS"] ?? builder.Configuration["urls"] ?? string.Empty)
    .Contains("https://", StringComparison.OrdinalIgnoreCase);

if (httpsConfigured)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
