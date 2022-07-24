namespace SF.BlogApi.Contracts;

public class GetUsersResponse
{
    public int UserAmount { get; set; }
    public UserView[] Users { get; set; }
}

public class UserView
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}
