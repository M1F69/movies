using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Data.Entities;

public class UserEntity
{
    public string NickName { get; set; } = "";
    
    public string FullName { get; set; } = "";
    
    public string Password { get; set; } = "";
    
    public string Mail { get; set; } = "";
    
}

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");


        builder.Property(p => p.NickName).HasColumnName("user_nickname");
        builder.Property(p => p.FullName).HasColumnName("user_fullname");
        builder.Property(p => p.Password).HasColumnName("user_password");
        builder.Property(p => p.Mail).HasColumnName("user_mail");

    }
}