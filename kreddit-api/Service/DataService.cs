using kreddit_api.Data;
using Microsoft.EntityFrameworkCore;
using shared.Model;

namespace kreddit_api.Service;

public class DataService
{
    private KredditContext db { get; }

    public DataService(KredditContext db)
    {
        this.db = db;
    }

    // Fylder testdata på, hvis databasen er tom
    public void SeedData()
    {
        if (!db.Posts.Any())
        {
            var post = new Post { Title = "Første tråd", Content = "Hej Kreddit!", Author = "Marius" };
            post.Comments.Add(new Comment { Content = "Første kommentar", Author = "Mads" });
            db.Posts.Add(post);
            db.SaveChanges();
        }
    }

    public List<Post> GetPosts()
    {
        return db.Posts.OrderByDescending(p => p.Created).ToList();
    }

    public Post? GetPost(int id)
    {
        return db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == id);
    }
}