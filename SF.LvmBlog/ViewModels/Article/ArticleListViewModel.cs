using SF.BlogData.Models;

namespace SF.LvmBlog.ViewModels;

public class ArticleListViewModel
{
    public IEnumerable<ArticleViewModel> Articles { get; set; }
    public PageViewModel PageViewModel { get; set; }
    public SortViewModel SortViewModel { get; set; }
    public string SearchText { get; set; }
}