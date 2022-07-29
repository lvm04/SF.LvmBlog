using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.Components;
public class AuthUserViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    public AuthUserViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
        AuthModel authModel = new AuthModel();

        authModel.Login = UserClaimsPrincipal.Identity.Name;
        var user = await userRepo.GetByLogin(authModel.Login);
        authModel.Name = user?.Name;

        return View(authModel);
    }
}
