using System.Data.Entity;

namespace MvcBlog.Models
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext() : base("BlogDbConnection") { }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
