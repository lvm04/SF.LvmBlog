using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.Controllers
{
    [AuthorizeRoles(Roles.Admin)]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;

            var _users = await userRepo.GetAllWithRoles();
            var users = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(_users);

            ViewData["Header"] = "Пользователи";
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил список пользователей");
            return View(users);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Search([FromQuery] string searchText)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;

            IEnumerable<User> _users;
            if (string.IsNullOrEmpty(searchText))
            {
                ViewData["Header"] = $"Пользователи";
                _users = await userRepo.GetAllWithRoles();
            }
            else
            {
                ViewData["Header"] = $"Пользователи по фильтру \"{searchText}\"";
                _users = await userRepo.GetByText(searchText);
            }

            var users = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(_users);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] отфильтровал пользователей");
            return View("Index", users);
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return View();
        }

        /// <summary>
        /// Форма редактирования пользователя
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var oldUser = await userRepo.GetById(id);
            if (oldUser == null)
                return View("UserError", $"Ошибка: Пользователя с идентификатором {id} не существует.");

            var user = _mapper.Map<UserCreateViewModel>(oldUser);

            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var _roles = await roleRepo.GetAll();

            user.Roles = _roles.Select(t =>
                new OptionViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    isChecked = oldUser.Roles.Select(r => r.Name).Contains(t.Name)
                }).ToArray();

            ViewData["Title"] = "Редактирование пользователя";
            ViewData["Header"] = "Редактирование пользователя";
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил для редактирования пользователя с Id={id}");
            return View(user);
        }

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Edit([FromForm] UserCreateViewModel user)
        {
            if (ModelState.IsValid)
            {
                var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                var oldUser = await userRepo.GetById(user.Id);
                if (oldUser == null)
                    return View("UserError", $"Ошибка: Пользователя с идентификатором {user.Id} не существует.");

                var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;

                var newUser = _mapper.Map(user, oldUser);
                newUser.Roles = await GetRepoRoles(roleRepo, user.OptionNames);

                await userRepo.Update(newUser);
                _logger.LogInformation($"[{HttpContext.User.Identity.Name}] отредактировал пользователя с Id={user.Id}");
                return RedirectToAction("Index", "User");
            }
            return View(user);

        }

        [HttpGet]
        [AuthorizeRoles(Roles.Admin)]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var user = await userRepo.Get(id);
            if (user == null)
                return View("UserError", $"Ошибка: Пользователя с идентификатором {id} не существует.");

            await userRepo.Delete(user);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] удалил пользователя с Id={user.Id}");
            return RedirectToAction("Index", "User");
        }

        // Преобразование списка ролей
        private async Task<List<Role>> GetRepoRoles(RoleRepository repo, string[] options)
        {
            var roleList = new List<Role>();
            if (options == null || options.Length == 0)
                return roleList;

            foreach (var option in options)
            {
                var dbRole = await repo.GetByName(option);
                if (dbRole != null)
                    roleList.Add(dbRole);
            }
            return roleList;
        }
    }
}
