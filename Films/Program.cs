using Films.Contracts;
using Films.Data;
using Films.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddDbContext<MovieDbContext>(
        o => o.UseNpgsql("Host=84.54.44.140;Port=2567;Username=postgres;Password=12345678;Database=Films;Command Timeout=100")
    );

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<MovieEntity>("Movies");
modelBuilder.EntitySet<AuthorEntity>("Authors");
modelBuilder.EntitySet<UserEntity>("Users");
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

app.MapPost("/sign-in", (SignInContract payload, MovieDbContext dbContext) =>
{
    var user = dbContext.Set<UserEntity>().FirstOrDefault(x => x.Password == payload.Password && x.NickName == payload.Username);

    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/viewed", (ViewedContract payload, MovieDbContext dbContext) =>
{
    var user = dbContext.Set<UserEntity>().First(x => x.Id == payload.UserId);
    var movie = dbContext.Set<MovieEntity>().First(x => x.Id == payload.MovieId);
    
    user.Viewed.Add(movie);

    dbContext.SaveChanges();
    
    return user;
});


app.Run();