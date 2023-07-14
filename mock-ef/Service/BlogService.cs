namespace mock_ef.Service;

using Microsoft.EntityFrameworkCore;
using mock_ef.data;
using mock_ef.data.Model;

public class BlogService
{
    private MockDbContext _context;

    public BlogService(MockDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Blog>> GetBlogs()
    {
        return await _context.Blogs.ToListAsync();
    }
    
    public async Task<Blog> GetBlog(int id)
    {
        return await _context.Blogs.FirstOrDefaultAsync(b => b.BlogId == id);
    }
    
    public Blog AddBlog(string name, string url)
    {
        var blog = new Blog { Name = name, Url = url };
        _context.Blogs.Add(blog);
        _context.SaveChanges();

        return blog;
    }
}