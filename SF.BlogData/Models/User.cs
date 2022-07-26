﻿namespace SF.BlogData.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Role> Roles { get; set; } = new();
    public List<Article> Articles { get; set; } = new();
}
