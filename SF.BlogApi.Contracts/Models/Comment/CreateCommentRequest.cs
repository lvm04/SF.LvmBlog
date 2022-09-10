namespace SF.BlogApi.Contracts;

public class CreateCommentRequest : EditCommentRequest
{
    public int ArticleId { get; set; }
}
