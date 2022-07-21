using Microsoft.AspNetCore.Mvc;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;

namespace SF.BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Просмотр списка пользователей
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;

            var users = await userRepo.GetAll();

            return StatusCode(200, users);          // Response.StatusCode = StatusCodes.Status400BadRequest; return new JsonResult(users);
        }
    }
}
