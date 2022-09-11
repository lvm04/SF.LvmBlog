using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SF.BlogApi.Contracts;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using System.IdentityModel.Tokens.Jwt;
using System;
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
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserRequest> _validator;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateUserRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
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
        [Route("Add")]
        public async Task<IActionResult> Create(CreateUserRequest user)
        {
            var validateResult = await _validator.ValidateAsync(user);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

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
        [Route("[action]/{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]CreateUserRequest user)
        {
            var validateResult = await _validator.ValidateAsync(user);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

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
        /// Редактирование ролей пользователя
        /// </summary>
        [HttpPut]
        [Route("[action]/{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> EditRoles([FromRoute] int id, [FromBody] string[] roles, [FromServices] ApplicationDbContext db)
        {
            // Сбой при работе с двумя сущностями через репозитории
            // https://entityframeworkcore.com/knowledge-base/52718652/ef-core-sqlite---sqlite-error-19---unique-constraint-failed

            var user = await db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return StatusCode(400, $"Ошибка: Пользователь с идентификатором {id} не существует.");

            var roleList = new List<Role>();
            foreach (var role in roles)
            {
                var dbRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == role);
                if (dbRole != null)
                    roleList.Add(dbRole);
            }

            user.Roles = roleList;
            //db.Log = Console.WriteLine;
            db.SaveChanges();

            return StatusCode(200, _mapper.Map<UserView>(user));
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        [HttpDelete]
        [Route("[action]/{id}")]
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
                //await CookieAuthenticate(user);
                //return StatusCode(200, $"Вы успешно вошли на сайт");
                return StatusCode(200, JwtAuthenticate(user));
            }
            else
            {
                return StatusCode(401, $"Ошибка: Неверный пароль.");
            }
        }

        private object JwtAuthenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = $"Bearer {encodedJwt}",
                login = user.Login,
                email = user.Email
            };
            return response;
        }

        private async Task CookieAuthenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
