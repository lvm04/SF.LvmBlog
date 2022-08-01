using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class ArticleCreateViewModel
{
    public string Title { get; set; }
    public string Text { get; set; }
    public bool[] Tags { get; set; }
    public TagViewModel[] TagNames { get; set; }
}

public class TagViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool isChecked { get; set; }
}