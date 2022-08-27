using SF.BlogData.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.LvmBlog.ViewModels;

public class ArticleCreateViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Не указан заголовок")]
    [StringLength(250, MinimumLength = 3, ErrorMessage = "Длина заголовка должна быть от 3 до 250 символов")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Напишите текст")]
    public string Text { get; set; }
    public string[] OptionNames { get; set; }               // теги, приходящие от клиента
    public OptionViewModel[] Tags { get; set; }            // теги, уходящие клиенту для настройки чекбоксов
}
