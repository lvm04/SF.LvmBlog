using Microsoft.AspNetCore.Authorization;
using SF.BlogData.Models;

namespace SF.LvmBlog.Controllers;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Roles[] roles) : base()
    {
        Roles = String.Join(",", roles.Select(r => r.ToString()));
    }
}
