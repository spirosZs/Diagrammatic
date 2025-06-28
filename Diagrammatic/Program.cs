using Diagrammatic_test.Components;
using Diagrammatic_test.Services;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//builder.Services.AddScoped<DiagrammaticService>();

builder.Services.AddHttpClient("DiagrammaticClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:8083/gameHub");
});
builder.Services.AddSingleton<RefreshService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<RefreshService>());
// Authentication services
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProviderService>();
builder.Services.AddAuthorizationCore();

// Add Blazored.Toast services
builder.Services.AddBlazoredToast();
builder.Services.AddRazorPages();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = new PathString("/login");
            options.AccessDeniedPath = new PathString("/error");
        });

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

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
