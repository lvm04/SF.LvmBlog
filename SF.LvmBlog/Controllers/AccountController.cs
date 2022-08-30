using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;
using System.Security.Claims;
using System.Web;

namespace SF.LvmBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AccountController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserRequest user)
        {
            if (ModelState.IsValid)
            {
                var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                var _user = await userRepo.GetByLogin(user.Login);
                if (_user == null)
                {
                    var roleRepo = _unitOfWork.GetRepository<Role>(false);
                    var userRole = await roleRepo.Get(2);

                    var dbUser = _mapper.Map<User>(user);
                    dbUser.Roles.Add(userRole);
                    await userRepo.Create(dbUser);

                    await Authenticate(dbUser);             // аутентификация

                    _logger.LogInformation($"[{dbUser.Login}] регистрация нового пользователя с Id={dbUser.Id}");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogError($"[] Пользователь с логином {user.Login} уже существует.");
                    ModelState.AddModelError("", $"Ошибка: Пользователь с таким логином уже существует.");
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                User _user = await userRepo.CheckCredentials(user.Login, user.Password);
                if (_user != null)
                {
                    await Authenticate(_user);
                    _logger.LogInformation($"[{_user.Login}] успешная аутентификация");

                    string returnUrl = GetQueryParam(HttpContext.Request.Headers["Referer"].ToString(), "ReturnUrl");
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }

                _logger.LogError($"[] Некорректные логин и(или) пароль");
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(user);
        }

        private string GetQueryParam(string uriStr, string paramName)
        {
            Uri _uri = new Uri(uriStr);
            string result = HttpUtility.ParseQueryString(_uri.Query).Get(paramName);
            return result;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] вышел из учетной записи");
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Value.ToString()));
            }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public IActionResult Manage()
        {
            return View();
        }
    }
}