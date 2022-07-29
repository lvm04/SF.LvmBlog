using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;
public class CreateUserRequest
{
    [Required(ErrorMessage = "Не указан логин")]
    public string Login { get; set; }

    public string Name { get; set; }

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароль введен неверно")]
    public string ConfirmPassword { get; set; }

    public string Email { get; set; }
}

public class LoginRequest
{
    [Required(ErrorMessage = "Не указан логин")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

// Для отображения на странице вошедшего пользователя
public class AuthModel
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
}