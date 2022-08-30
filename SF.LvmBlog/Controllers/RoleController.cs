using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.Controllers
{
    [AuthorizeRoles(Roles.Admin)]
    [Route("{controller}")]
    public class RoleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;

            var _roles = await roleRepo.GetAll();
            var roles = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(_roles);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил список ролей");
            return View(roles);
        }

        /// <summary>
        /// Форма добавления роли
        /// </summary>
        [HttpGet]
        [Route("New")]
        public IActionResult Create()
        {
            ViewSettings(true);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил форму создания роли");
            return View(new RoleViewModel());
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] RoleViewModel role)
        {
            if (!ModelState.IsValid)
            {
                ViewSettings(true);
                return View(role);
            }

            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var newRole = _mapper.Map<Role>(role);
            await roleRepo.Create(newRole);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] создал новую роль с Id={newRole.Id}");
            return RedirectToAction("Index");
        }

        private void ViewSettings(bool isCreate)
        {
            if (isCreate)
            {
                ViewData["Title"] = "Роль";
                ViewData["Header"] = "Новая роль";
                ViewData["Method"] = "Create";
            }
            else
            {
                ViewData["Title"] = "Роль";
                ViewData["Header"] = "Редактирование роли";
                ViewData["Method"] = "Edit";
            }
        }

        /// <summary>
        /// Форма редактирования роли
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var dbRole = await roleRepo.Get(id);
            if (dbRole == null)
            {
                return View("UserError", $"Ошибка: Роль с идентификатором {id} не существует.");
            }

            var role = _mapper.Map<RoleViewModel>(dbRole);

            ViewSettings(false);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил для редактирования роль с Id={id}");
            return View("Create", role);
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        [HttpPost]
        [Route("{action}")]
        public async Task<IActionResult> Edit([FromForm] RoleViewModel role)
        {
            if (!ModelState.IsValid)
            {
                ViewSettings(false);
                return View("Create", role);
            }

            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var oldRole = await roleRepo.Get(role.Id);
            if (oldRole == null)
            {
                return View("UserError", $"Ошибка: Роль с идентификатором {role.Id} не существует.");
            }

            var newRole = _mapper.Map(role, oldRole);

            await roleRepo.Update(newRole);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] отредактировал роль с Id={newRole.Id}");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Удалить роль
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var role = await roleRepo.Get(id);
            if (role == null)
            {
                return View("UserError", $"Ошибка: Роль с идентификатором {id} не существует.");
            }

            await roleRepo.Delete(role);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] удалил роль с Id={id}");
            return RedirectToAction("Index");
        }
    }
}