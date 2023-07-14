namespace mock_ef.data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mock_ef.data.Model;

public class PostConfiguration: IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(100);
        builder.Property(p => p.Content)
               .IsRequired()
               .HasMaxLength(1000);
        builder.Property(p => p.BlogId)
               .IsRequired();
        builder.HasIndex(p => p.BlogId);
    }
}