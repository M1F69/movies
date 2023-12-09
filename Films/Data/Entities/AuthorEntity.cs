using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Data.Entities;

public class AuthorEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string SurName { get; set; } = "";
    public string LastName { get; set; } = "";
    public ushort BornDate { get; set; } = 0;
    public virtual ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}

public class AuthorEntityConfiguration: IEntityTypeConfiguration<AuthorEntity>
{
    public void Configure(EntityTypeBuilder<AuthorEntity> builder)
    {
        builder.ToTable("authors", schema: "movie");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("author_id");
        builder.Property(p => p.Name).HasColumnName("author_name");
        builder.Property(p => p.LastName).HasColumnName("author_description");
        builder.Property(p => p.SurName).HasColumnName("author_year");
        builder.Property(p => p.BornDate).HasColumnName("author_type");

    }
}