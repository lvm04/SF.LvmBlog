namespace SF.BlogApi.Contracts;

public class CreateCommentRequest
{
    public int ArticleId { get; set; }
    public string Text { get; set; }
}
