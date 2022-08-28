using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class TagViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Напишите название тега")]
    public string Name { get; set; }
}
