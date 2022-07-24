namespace SF.BlogApi.Contracts;

public class EditCommentRequest
{
    public int AuthorId { get; set; }
    public int ArticleId { get; set; }
    public string Text { get; set; }
}
