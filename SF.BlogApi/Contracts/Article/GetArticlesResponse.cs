namespace SF.BlogApi.Contracts;

public class GetArticlesResponse
{
    public int ArticlesAmount { get; set; }
    public ArticleView[] Articles { get; set; }
}

public class ArticleView
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string[] Tags { get; set; }
    public string[] Comments { get; set; }
}
