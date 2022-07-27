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
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Просмотр всех комментариев к статье
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetByArticleId([FromRoute] int id)
        {
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            var _comments = await commentRepo.GetByArticleId(id);
            var comments = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentView>>(_comments);
            var commentsResponse = new GetCommentsResponse
            {
                CommentsAmount = comments.Count(),
                Comments = comments.ToArray()
            };

            return StatusCode(200, commentsResponse);
        }

        /// <summary>
        /// Получить комментарий по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var _comment = await commentRepo.GetById(id);
            if (_comment == null)
                return StatusCode(400, $"Ошибка: Комментарий с идентификатором {id} не существует.");
            var comment = _mapper.Map<CommentView>(_comment);
            
            return StatusCode(200, comment);  
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
            var currentUser = await HttpContext.GetCurrentUser();
            dbComment.AuthorId = currentUser.Id;
            await commentRepo.Create(dbComment);

            return StatusCode(200, _mapper.Map<CommentView>(dbComment));
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditCommentRequest comment)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var oldComment = await commentRepo.Get(id);

            if (oldComment == null)
                return StatusCode(400, $"Ошибка: Комментарий с идентификатором {id} не существует.");

            // Проверяем может ли текущий пользователь редактировать данный комментарий
            // Для этого он должен быть автором или принадлежать к группе admin или moderator
            if (oldComment.AuthorId != currentUser.Id 
                && currentUser.Roles.FirstOrDefault(r => r.Name == "admin" ) == null
                && currentUser.Roles.FirstOrDefault(r => r.Name == "moderator") == null)
                return StatusCode(400, $"Ошибка: Вы не имеете право редактировать этот комментарий.");

            var newComment = _mapper.Map(comment, oldComment);
            await commentRepo.Update(newComment);

            return StatusCode(200, _mapper.Map<CommentView>(newComment));
        }


        /// <summary>
        /// Удаление комментария
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var comment = await commentRepo.Get(id);
            if (comment == null)
                return StatusCode(400, $"Ошибка: Комментарий с идентификатором {id} не существует.");

            await commentRepo.Delete(comment);

            return StatusCode(200, $"Комментарий с ID {comment.Id} удален");
        }



    }
}
