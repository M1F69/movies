using Films.Data;
using Films.Data.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddDbContext<MovieDbContext>(
        o => o.UseNpgsql("Host=127.0.0.1;Port=2567;Username=postgres;Password=12345678;Database=Films;Command Timeout=100")
    );

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<MovieEntity>("Movies");
modelBuilder.EntitySet<AuthorEntity>("Authors");
modelBuilder.EnableLowerCamelCase();

builder
    .Services
    .AddControllers()
    .AddOData(
        o => o.EnableQueryFeatures().AddRouteComponents(modelBuilder.GetEdmModel())
    );

var app = builder.Build();

app.UseRouting();
app.UseODataRouteDebug();
app.UseDbContext();

app.MapControllers();

app.MapGet("/reload", () => {
    System.Diagnostics.Process.Start("/etc/init.d/postgresql", "restart");
});

app.Run();