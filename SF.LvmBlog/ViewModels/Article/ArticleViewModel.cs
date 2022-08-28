using SF.BlogData.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class ArticleViewModel
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public User Author { get; set; }
    public int NumberViews { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    [Required(ErrorMessage = "Напишите текст")]
    public string CommentText { get; set; }
}