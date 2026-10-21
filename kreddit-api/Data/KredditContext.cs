using Microsoft.EntityFrameworkCore;
using shared.Model;

namespace kreddit_api.Data;

public class KredditContext : DbContext
{
    public KredditContext(DbContextOptions<KredditContext> options) : base(options) { }

    
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    
}