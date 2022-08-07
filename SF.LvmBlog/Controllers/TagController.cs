using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using SF.LvmBlog.ViewModels;
using System.Web;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using Microsoft.AspNetCore.Authorization;

namespace SF.LvmBlog.Controllers
{
    [Route("{controller}")]
    public class TagController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;

            var _tags = await tagRepo.GetAll();
            var tags = _mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(_tags);

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
            ViewData["Title"] = "Тег";
            ViewData["Header"] = "Новый тег";
            ViewData["Method"] = "Create";
         
            return View(new TagViewModel());
        }

        /// <summary>
        /// Создание тега
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] TagViewModel tag)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var newTag = _mapper.Map<Tag>(tag);
            await tagRepo.Create(newTag);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Форма редактирования тега
        /// </summary>
        [HttpGet]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var dbTag = await tagRepo.Get(id);
            if (dbTag == null)
            {
                return NotFound($"Ошибка: Тег с идентификатором {id} не существует.");
            }

            var tag = _mapper.Map<TagViewModel>(dbTag);
            ViewData["Title"] = "Тег";
            ViewData["Header"] = "Редактирование тега";
            ViewData["Method"] = "Edit";

            return View("Create", tag);
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [HttpPost]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        [Route("{action}")]
        public async Task<IActionResult> Edit([FromForm] TagViewModel tag)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var oldTag = await tagRepo.Get(tag.Id);
            if (oldTag == null)
            {
                return NotFound($"Ошибка: Тег с идентификатором {tag?.Id} не существует.");
            }

            var newTag = _mapper.Map(tag, oldTag);

            await tagRepo.Update(newTag);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Удалить тег
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.Get(id);
            if (tag == null)
            {
                return NotFound($"Ошибка: Тег с идентификатором {id} не существует.");
            }

            await tagRepo.Delete(tag);
            return RedirectToAction("Index");
        }
    }
}