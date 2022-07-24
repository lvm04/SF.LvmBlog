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
    /// Комментарии к статье
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        /// <summary>
        /// Получить комментарий по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var _article = await articleRepo.GetById(id);
            if (_article == null)
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");
            var article = _mapper.Map<ArticleView>(_article);
            
            return StatusCode(200, article);  
        }


        /// <summary>
        /// Создание комментария
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Create(CreateCommentRequest comment)
        {
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var dbComment = _mapper.Map<Comment>(comment);
            await commentRepo.Create(dbComment);

            return StatusCode(200, _mapper.Map<CommentView>(dbComment));
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]CreateCommentRequest comment)
        {
            var currentUserLogin = HttpContext.User.Identity.Name;
            var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
            var currentUser = await userRepo.GetByLogin(currentUserLogin);

            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var oldComment = await commentRepo.Get(id);

            if (oldComment == null)
                return StatusCode(400, $"Ошибка: Комментарий с идентификатором {id} не существует.");

            // Проверяем может ли текущий пользователь редактировать данный комментарий
            // Для этого он должен быть автором или принадлежать к группе admin или moderator
            //if (comment.AuthorId != currentUser.Id 
            //    && currentUser.Roles.FirstOrDefault(r => r.Name == "admin" ) == null
            //    && currentUser.Roles.FirstOrDefault(r => r.Name == "moderator") == null)
            //    return StatusCode(400, $"Ошибка: Вы не имеете право редактировать этот комментарий.");

            var newComment = _mapper.Map(comment, oldComment);
            await commentRepo.Update(newComment);

            return StatusCode(200, _mapper.Map<CommentView>(newComment));
        }

        /// <summary>
        /// Удаление комментария
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var comment = await commentRepo.Get(id);
            if (comment == null)
                return StatusCode(400, $"Ошибка: Комментарий с идентификатором {id} не существует.");

            await commentRepo.Delete(comment);

            return StatusCode(200, $"Комментарий с ID {comment.Id}) удален");
        }



    }
}
