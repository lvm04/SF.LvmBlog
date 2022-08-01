using SF.BlogData.Models;
using SF.BlogData.Repository;

namespace SF.LvmBlog.Controllers;

public static class UserExtentions
{
    public static async Task<User> GetCurrentUser(this HttpContext httpContext)
    {
        var sp = httpContext.RequestServices;

        var currentUserLogin = httpContext.User.Identity.Name;
        var userRepo = sp.GetService<IRepository<User>>() as UserRepository;
        return await userRepo.GetByLogin(currentUserLogin);
    }
}
