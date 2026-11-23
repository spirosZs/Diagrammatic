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

// Revalidate rather than reuse. diagramInterop.js is this app's half of the bridge to the
// diagram editor, and App.razor is a component, so it cannot version the tag the way an MVC
// view does with asp-append-version. Without this, a browser that cached the file before a
// deploy keeps running the old bridge against the new editor — which does not look like a
// stale script, it looks like the feature is broken. "no-cache" still allows the cached copy
// to be reused, it just has to be checked first, and the ETag makes that a 304.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.CacheControl = "no-cache"
});
app.UseAntiforgery();

// Map Blazor components.
//
// AllowAnonymous is about *where* authorization happens, not whether it happens. Who the
// user is lives in the session-storage token that AuthStateProviderService reads, which
// only exists inside the circuit — the plain HTTP request that fetches the page carries
// nothing the server can identify. A page component's [Authorize] still becomes endpoint
// metadata though, so the authorization middleware tried to challenge every such request
// and, with no authentication scheme registered in this app, threw instead: a hard refresh
// (or a pasted link) on /create-exercise, /create-exam or /student-submissions answered
// 500 rather than showing anything. Clicking through from the nav bar never hit it, being
// routed inside the circuit — which is why it only ever bit on a reload.
//
// Letting the request through hands the decision to AuthorizeRouteView in Routes.razor,
// which does know the user: an unauthorized visitor gets the "Access Denied" card with a
// login link, a signed-in one gets the page. Nothing is exposed by serving the shell — the
// data behind these pages comes from the API, which authenticates every call by bearer
// token regardless of what this app renders.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();
