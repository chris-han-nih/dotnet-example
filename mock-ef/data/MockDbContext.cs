namespace mock_ef.data;

using Microsoft.EntityFrameworkCore;
using mock_ef.data.Configuration;
using mock_ef.data.Model;

public class MockDbContext: DbContext
{
    public MockDbContext()
    {
    }
    public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlogConfiguration());
        modelBuilder.ApplyConfiguration(new PostConfiguration());
    }
}