using RoboForge.Modules.Catalog.Infrastructure;
using RoboForge.Modules.Catalog.Infrastructure.Database;
using RoboForge.Modules.Catalog.UI;
using RoboForge.Modules.Production.Infrastructure;
using RoboForge.Modules.Production.Infrastructure.Database;
using RoboForge.Modules.Production.UI;
using RoboForge.Modules.Sales.Infrastructure;
using RoboForge.Modules.Sales.Infrastructure.Database;
using RoboForge.Modules.Sales.UI;
using RoboForge.Web.Database;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApiHostServices();

builder.AddCatalogModule();
builder.AddSalesModule();
builder.AddProductionModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyPendingMigrationsAsync<CatalogDbContext>();
    await app.ApplyPendingMigrationsAsync<SalesDbContext>();
    await app.ApplyPendingMigrationsAsync<ProductionDbContext>();
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
app.MapSalesModuleEndpoints();
app.MapProductionModuleEndpoints();

await app.RunAsync();
