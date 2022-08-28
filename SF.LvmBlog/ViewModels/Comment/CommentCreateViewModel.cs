using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class CommentCreateViewModel
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    [Required(ErrorMessage = "Напишите текст")]
    public string CommentText { get; set; }
}
