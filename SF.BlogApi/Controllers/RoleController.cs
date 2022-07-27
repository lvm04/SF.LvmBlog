using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SF.BlogApi.Contracts;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using System.Security.Claims;

namespace SF.BlogApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        /// <summary>
        /// Просмотр списка ролей
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var roles = await roleRepo.GetAll();
            var roleResponse = new
            {
                RolesAmount = roles.Count(),
                Roles = roles.ToArray()
            };

            return StatusCode(200, roleResponse);
        }

        /// <summary>
        /// Получить роль по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var role = await roleRepo.Get(id);
            if (role == null)
                return StatusCode(400, $"Ошибка: Роль с идентификатором {id} не существует.");

            return StatusCode(200, role);  
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(CreateRoleRequest role)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var _role = await roleRepo.GetByName(role?.Name);

            if (_role != null)
                return StatusCode(400, $"Ошибка: Роль с таким названием уже существует.");

            await roleRepo.Create(new Role { Name = role?.Name, Description = role?.Description  });

            return StatusCode(200, $"Роль \"{role.Name}\" создана");
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]CreateRoleRequest role)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var _role = await roleRepo.Get(id);
            if (_role == null)
                return StatusCode(400, $"Ошибка: Роль с идентификатором {id} не существует.");

            _role.Name = role.Name;
            _role.Description = role.Description;
            await roleRepo.Update(_role);
            return StatusCode(200, _role);
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role>() as RoleRepository;
            var role = await roleRepo.Get(id);
            if (role == null)
                return StatusCode(400, $"Ошибка: Роль с идентификатором {id} не существует.");

            await roleRepo.Delete(role);

            return StatusCode(200, $"Тег \"{role.Name}\" удален");
        }



    }
}
