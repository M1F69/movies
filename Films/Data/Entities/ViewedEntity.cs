using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Data.Entities;

public class ViewedEntity
{
    public Guid UserId { get; set; }
    
    public Guid MovieId { get; set; }

}

public class MovieUserEntityConfiguration : IEntityTypeConfiguration<ViewedEntity>
{
    public void Configure(EntityTypeBuilder<ViewedEntity> builder)
    {
        builder.ToTable("viewed", schema: "movie");

        builder.HasKey(p => new { p.UserId, p.MovieId });

        builder.Property(p => p.UserId).HasColumnName("viewed_user_id");
        builder.Property(p => p.MovieId).HasColumnName("viewed_movie_id");
    }
}