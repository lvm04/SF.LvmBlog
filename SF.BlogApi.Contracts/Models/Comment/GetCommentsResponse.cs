namespace SF.BlogApi.Contracts;

public class GetCommentsResponse
{
    public int CommentsAmount { get; set; }
    public CommentView[] Comments { get; set; }
}


