using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class CommentCreateViewModel
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string Text { get; set; }
}
