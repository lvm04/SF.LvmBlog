namespace SF.BlogData.Models;

public class Comment
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public string Text { get; set; }
   
}
