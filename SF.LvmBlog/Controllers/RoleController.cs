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
    public class RoleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;

            var _roles = await roleRepo.GetAll();
            var roles = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(_roles);

            return View(roles);
        }

        /// <summary>
        /// Форма добавления роли
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("New")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Роль";
            ViewData["Header"] = "Новая роль";
            ViewData["Method"] = "Create";
         
            return View(new RoleViewModel());
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] RoleViewModel role)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var newRole = _mapper.Map<Role>(role);
            await roleRepo.Create(newRole);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Форма редактирования роли
        /// </summary>
        [HttpGet]
        [AuthorizeRoles(Roles.Admin)]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var dbRole = await roleRepo.Get(id);
            if (dbRole == null)
            {
                return NotFound($"Ошибка: Роль с идентификатором {id} не существует.");
            }

            var role = _mapper.Map<RoleViewModel>(dbRole);
            ViewData["Title"] = "Роль";
            ViewData["Header"] = "Редактирование роли";
            ViewData["Method"] = "Edit";

            return View("Create", role);
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        [HttpPost]
        [AuthorizeRoles(Roles.Admin)]
        [Route("{action}")]
        public async Task<IActionResult> Edit([FromForm] RoleViewModel role)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var oldRole = await roleRepo.Get(role.Id);
            if (oldRole == null)
            {
                return NotFound($"Ошибка: Роль с идентификатором {role.Id} не существует.");
            }

            var newRole = _mapper.Map(role, oldRole);

            await roleRepo.Update(newRole);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Удалить роль
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        [AuthorizeRoles(Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var role = await roleRepo.Get(id);
            if (role == null)
            {
                return NotFound($"Ошибка: Роль с идентификатором {id} не существует.");
            }

            await roleRepo.Delete(role);
            return RedirectToAction("Index");
        }
    }
}