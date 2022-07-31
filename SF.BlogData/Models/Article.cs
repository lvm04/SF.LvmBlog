namespace SF.BlogData.Models;

public class Article
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Title { get; set; }
    public int? AuthorId { get; set; }
    public User Author { get; set; }
    public string Text { get; set; }
    public int NumberViews { get; set; }    // Кол-во просмотров
    public List<Comment> Comments { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
}