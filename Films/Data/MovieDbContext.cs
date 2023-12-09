using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Films.Data;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("lo");
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}