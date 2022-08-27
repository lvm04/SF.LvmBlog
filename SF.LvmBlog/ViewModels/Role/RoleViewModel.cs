using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class RoleViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Напишите название роли")]
    public string Name { get; set; }
    public string Description { get; set; }
}
