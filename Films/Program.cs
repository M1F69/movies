using Films.Contracts;
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
    var user = dbContext.Set<UserEntity>().Include(x => x.Viewed).FirstOrDefault(x => x.Password == payload.Password && x.NickName == payload.Username);

    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/viewed", (ViewedContract payload, MovieDbContext dbContext) =>
{
    var user = dbContext.Set<UserEntity>().Include(x => x.Viewed).First(x => x.Id == payload.UserId);
    var movie = dbContext.Set<MovieEntity>().First(x => x.Id == payload.MovieId);
    
    user.Viewed.Add(movie);

    dbContext.SaveChanges();
    
    return user;
});

app.MapPost("/unviewed", (ViewedContract payload, MovieDbContext dbContext) =>
{
    dbContext.Set<ViewedEntity>().Remove(
        dbContext.Set<ViewedEntity>().First(x => x.UserId == payload.UserId && x.MovieId == payload.MovieId)
    );
    dbContext.SaveChanges();
    return dbContext.Set<UserEntity>().Include(x => x.Viewed).First(x => x.Id == payload.UserId);
});



app.Run();