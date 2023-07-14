namespace mock_ef_test;

using Microsoft.EntityFrameworkCore;
using mock_ef.data;
using mock_ef.data.Model;
using mock_ef.Service;
using Moq;

public class NonQueryTest
{
    [Fact]
    public void CreateBlog_saves_a_blog_via_context()
    {
        var mockSet = new Mock<DbSet<Blog>>();
        var mockContext = new Mock<MockDbContext>();
        mockContext.Setup(m => m.Blogs).Returns(mockSet.Object);
        
        var service = new BlogService(mockContext.Object);
        service.AddBlog(name: "Test Blog",
                                      url: "http://test.com");

        mockSet.Verify(m => m.Add(It.IsAny<Blog>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }
}