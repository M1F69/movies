using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Data.Entities;

public class BlobEntity
{
    public Guid Id { get; set; }

    public long LoId { get; set; }
    
    public string Name { get; set; } = "";
    
    public string MimeType { get; set; } = "";
    
    public long Size { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}

public class MovieImageEntityConfiguration: IEntityTypeConfiguration<BlobEntity>
{
    public void Configure(EntityTypeBuilder<BlobEntity> builder)
    {
        builder.ToTable("blobs", schema: "movie");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("blob_id");
        builder.Property(p => p.Name).HasColumnName("blob_name");
        builder.Property(p => p.MimeType).HasColumnName("blob_mime_type");
        builder.Property(p => p.Size).HasColumnName("blob_size");
        builder.Property(p => p.CreatedAt).HasColumnName("blob_created_at");
        builder.Property(p => p.LoId).HasColumnName("lo_id").HasColumnType("lo");

    }
}