namespace shared.Model;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}