using Films.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Data.Entities;

public class MovieEntity
{
    public Guid Id { get; set; }

    public Guid? AuthorId { get; set; }
    
    public Guid? ImageId { get; set; }
    
    public string? ImageHRef { get; set; }

    public string Name { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public bool Viewed { get; set; } = false;

    public ushort Year { get; set; } = 0;

    public MovieType Type { get; set; } = MovieType.Default;
    
    public MovieGenreType Genre { get; set; } = MovieGenreType.ActionMovie;
    
    public virtual AuthorEntity? Author { get; set; }
    
    public virtual BlobEntity? Image { get; set; }
}

public class MovieEntityConfiguration: IEntityTypeConfiguration<MovieEntity>
{
    public void Configure(EntityTypeBuilder<MovieEntity> builder)
    {
        builder.ToTable("movies", schema: "movie");

        builder.HasKey(p => p.Id);
        
        builder.Ignore(p => p.ImageHRef);

        builder.Property(p => p.Id).HasColumnName("movie_id");
        builder.Property(p => p.AuthorId).HasColumnName("movie_author_id");
        builder.Property(p => p.ImageId).HasColumnName("movie_image_id");
        builder.Property(p => p.Name).HasColumnName("movie_name");
        builder.Property(p => p.Description).HasColumnName("movie_description");
        builder.Property(p => p.Viewed).HasColumnName("movie_viewed");
        builder.Property(p => p.Year).HasColumnName("movie_year");
        builder.Property(p => p.Type).HasColumnName("movie_type");
        builder.Property(p => p.Genre).HasColumnName("movie_genre");
        
        builder
            .HasOne(p => p.Author)
            .WithMany(p => p.Movies)
            .HasForeignKey(p => p.AuthorId);

        builder
            .HasOne(p => p.Image)
            .WithOne()
            .HasForeignKey<MovieEntity>(p => p.ImageId);
        
    }
}