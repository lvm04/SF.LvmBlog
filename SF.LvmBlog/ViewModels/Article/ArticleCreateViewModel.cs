using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class ArticleCreateViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string[] Tags { get; set; }              // теги, приходящие от клиента
    public OptionViewModel[] TagNames { get; set; }    // теги, уходящие клиенту для настройки чекбоксов
}
