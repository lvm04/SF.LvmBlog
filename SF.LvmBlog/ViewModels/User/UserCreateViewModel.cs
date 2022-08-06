using SF.BlogData.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class UserCreateViewModel
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароль введен неверно")]

    public string ConfirmPassword { get; set; }
    public string[] OptionNames { get; set; }                   // роли, приходящие от клиента
    public OptionViewModel[] Roles { get; set; }                // роли, уходящие клиенту для настройки чекбоксов
}
