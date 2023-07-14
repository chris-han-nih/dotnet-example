namespace mock_ef.data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mock_ef.data.Model;

public class BlogConfiguration: IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.Property(b => b.Name)
               .IsRequired()
               .HasMaxLength(100);
        builder.Property(b => b.Url)
               .HasMaxLength(300);
        builder.HasMany(b => b.Posts);
    }
}