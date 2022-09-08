namespace SF.BlogApi.Contracts;

public class CreateUserRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
