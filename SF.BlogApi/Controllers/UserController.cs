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
    /// <summary>
    /// Пользователи
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Просмотр списка пользователей
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;

            var _users = await userRepo.GetAllWithRoles();
            var users = _mapper.Map<IEnumerable<User>, IEnumerable<UserView>>(_users);
            var userResponse = new GetUsersResponse
            {
                UserAmount = users.Count(),
                Users = users.ToArray()
            };

            return StatusCode(200, userResponse);          // Response.StatusCode = StatusCodes.Status400BadRequest; return new JsonResult(users);
        }

        /// <summary>
        /// Получить пользователя по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var _user = await userRepo.GetById(id);
            if (_user == null)
                return StatusCode(400, $"Ошибка: Пользователь с идентификатором {id} не существует.");
            var user = _mapper.Map<UserView>(_user);
            
            return StatusCode(200, user);  
        }

        /// <summary>
        /// Получить пользователя по логину
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetByLogin([FromQuery]string login)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var _user = await userRepo.GetByLogin(login);
            if (_user == null)
                return StatusCode(400, $"Ошибка: Пользователь с именем {login} не существует.");
            var user = _mapper.Map<UserView>(_user);

            return StatusCode(200, user);
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateUserRequest user)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var _user = await userRepo.GetByLogin(user?.Login);
            if (_user != null)
                return StatusCode(400, $"Ошибка: Пользователь с таким именем уже существует.");

            var roleRepo = _unitOfWork.GetRepository<Role>(false);
            var userRole = await roleRepo.Get(2);
            var dbUser = _mapper.Map<User>(user);
            dbUser.Roles.Add(userRole);
            await userRepo.Create(dbUser);

            return StatusCode(200, _mapper.Map<UserView>(dbUser));
        }

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        [HttpPut]
        [Route("{id}")]
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

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
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


        /// <summary>
        /// Аутентификация
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var user = await userRepo.GetByLogin(request.Login);

            if (user == null)
                return StatusCode(401, $"Ошибка: Пользователь с логином {request.Login} не существует.");

            if (!String.IsNullOrEmpty(request.Password) && request.Password == user.Password)
            {
                await Authenticate(user);
                return StatusCode(200, $"Вы успешно вошли на сайт");
            }
            else
            {
                return StatusCode(401, $"Ошибка: Неверный пароль.");
            }

        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
