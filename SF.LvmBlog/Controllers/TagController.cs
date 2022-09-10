using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.Controllers
{
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TagController> _logger;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TagController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;

            var _tags = await tagRepo.GetAll();
            var tags = _mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(_tags);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил список тегов");
            return View(tags);
        }

        /// <summary>
        /// Форма добавления тега
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("New")]
        public IActionResult Create()
        {
            ViewSettings(true);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил форму создания тега");
            return View(new TagViewModel());
        }

        private void ViewSettings(bool isCreate)
        {
            if (isCreate)
            {
                ViewData["Title"] = "Тег";
                ViewData["Header"] = "Новый тег";
                ViewData["Method"] = "Create";
            }
            else
            {
                ViewData["Title"] = "Тег";
                ViewData["Header"] = "Редактирование тега";
                ViewData["Method"] = "Edit";
            }
        }

        /// <summary>
        /// Создание тега
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] TagViewModel tag)
        {
            if (!ModelState.IsValid)
            {
                ViewSettings(true);
                return View("Create", tag);
            }

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var newTag = _mapper.Map<Tag>(tag);
            await tagRepo.Create(newTag);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] создал новый тег с Id={newTag.Id}");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Форма редактирования тега
        /// </summary>
        [HttpGet]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var dbTag = await tagRepo.Get(id);
            if (dbTag == null)
            {
                return View("UserError", $"Ошибка: Тег с идентификатором {id} не существует.");
            }

            var tag = _mapper.Map<TagViewModel>(dbTag);
            ViewSettings(false);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил для редактирования тег с Id={id}");
            return View("Create", tag);
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [HttpPost]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        [Route("[action]")]
        public async Task<IActionResult> Edit([FromForm] TagViewModel tag)
        {
            if (!ModelState.IsValid)
            {
                ViewSettings(false);
                return View("Create", tag);
            }

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var oldTag = await tagRepo.Get(tag.Id);
            if (oldTag == null)
            {
                return View("UserError", $"Ошибка: Тег с идентификатором {tag.Id} не существует.");
            }

            var newTag = _mapper.Map(tag, oldTag);
            await tagRepo.Update(newTag);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] отредактировал тег с Id={newTag.Id}");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Удалить тег
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.Get(id);
            if (tag == null)
            {
                return View("UserError", $"Ошибка: Тег с идентификатором {id} не существует.");
            }

            await tagRepo.Delete(tag);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] удалил тег с Id={id}");
            return RedirectToAction("Index");
        }
    }
}