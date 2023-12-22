using Microsoft.EntityFrameworkCore;

namespace Films.Data;

public static class DbContextExtensions
{
    public static WebApplication UseDbContext(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
        
        return app;
    }

}