using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class ArticleShortViewModel
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public User Author { get; set; }
    public int NumberViews { get; set; }
    public List<Tag> Tags { get; set; } = new();
}