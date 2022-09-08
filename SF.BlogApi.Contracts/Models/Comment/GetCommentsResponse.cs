namespace SF.BlogApi.Contracts;

public class GetCommentsResponse
{
    public int CommentsAmount { get; set; }
    public CommentView[] Comments { get; set; }
}

public class CommentView
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public int ArticleId { get; set; }
    public string Text { get; set; }
}
