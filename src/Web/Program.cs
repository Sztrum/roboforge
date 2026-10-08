using RoboForge.Modules.Catalog.Infrastructure;
using RoboForge.Modules.Catalog.Infrastructure.Database;
using RoboForge.Modules.Catalog.UI;
using RoboForge.Web.Database;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApiHostServices();

builder.AddCatalogModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyPendingMigrationsAsync<CatalogDbContext>();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/scalar"));

app.MapDefaultEndpoints();
app.MapCatalogModuleEndpoints();

await app.RunAsync();
