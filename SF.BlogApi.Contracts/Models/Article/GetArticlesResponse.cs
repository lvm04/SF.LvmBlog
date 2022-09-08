namespace SF.BlogApi.Contracts;

public class GetArticlesResponse
{
    public int ArticlesAmount { get; set; }
    public ArticleView[] Articles { get; set; }
}


