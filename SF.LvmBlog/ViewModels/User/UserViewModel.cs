using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Role> Roles { get; set; } = new();
}
