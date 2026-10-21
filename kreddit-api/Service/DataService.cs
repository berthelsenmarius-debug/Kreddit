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
    public Comment? CreateComment(int postId, string content, string author)
    {
        var post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
        if (post == null) return null;

        var comment = new Comment { Content = content, Author = author };
        post.Comments.Add(comment);
        db.SaveChanges();
        return comment;
    }
    public Post CreatePost(string title, string content, string? url, string author)
    {
        var post = new Post { Title = title, Content = content, Url = url, Author = author };
        db.Posts.Add(post);
        db.SaveChanges();
        return post;
    }
    
    public Post? VotePost(int id, bool up)
    {
        var post = db.Posts.FirstOrDefault(p => p.Id == id);
        if (post == null) return null;

        if (up) post.Upvotes++;
        else post.Downvotes++;

        db.SaveChanges();
        return post;
    }

    public Comment? VoteComment(int id, bool up)
    {
        var comment = db.Comments.FirstOrDefault(c => c.Id == id);
        if (comment == null) return null;

        if (up) comment.Upvotes++;
        else comment.Downvotes++;

        db.SaveChanges();
        return comment;
    }
}