using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SF.LvmBlog.ViewModels;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using System.Security.Claims;

namespace SF.BlogApi.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Index()
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;

            var _users = await userRepo.GetAllWithRoles();
            
            return View(_users);
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var _user = await userRepo.GetById(id);
            if (_user == null)
                return StatusCode(400, $"Ошибка: Пользователь с идентификатором {id} не существует.");
            var user = _mapper.Map<UserView>(_user);
            
            return StatusCode(200, user);  
        }

        
        [HttpGet]
        public async Task<IActionResult> GetByLogin([FromQuery]string login)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var _user = await userRepo.GetByLogin(login);
            if (_user == null)
                return StatusCode(400, $"Ошибка: Пользователь с именем {login} не существует.");
            var user = _mapper.Map<UserView>(_user);

            return StatusCode(200, user);
        }


        [HttpPut]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]CreateUserRequest user)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var oldUser = await userRepo.Get(id);
            if (oldUser == null)
                return StatusCode(400, $"Ошибка: Пользователь с идентификатором {id} не существует.");

            var newUser = _mapper.Map(user, oldUser);

            // Роли пока не редактируем
            await userRepo.Update(newUser);

            return StatusCode(200, _mapper.Map<UserView>(newUser));
        }

       
        [HttpDelete]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var user = await userRepo.Get(id);
            if (user == null)
                return StatusCode(400, $"Ошибка: Пользователь с идентификатором {id} не существует.");

            await userRepo.Delete(user);

            return StatusCode(200, $"Пользователь {user.Name} (ID {user.Id}) удален");
        }
        */
    }
}
