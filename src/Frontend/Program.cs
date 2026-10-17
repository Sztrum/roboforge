using RoboForge.Frontend.Api;
using RoboForge.Frontend.Components;
using RoboForge.Shared.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Aspire service discovery resolves the API's address from its resource name.
builder.Services.AddHttpClient<RoboForgeApiHttpClient>(client =>
    client.BaseAddress = new Uri($"https+http://{AspireResourceNames.WebApi}"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

await app.RunAsync();
