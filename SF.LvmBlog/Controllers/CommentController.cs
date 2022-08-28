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
using System.Data;

namespace SF.LvmBlog.Controllers
{
    [Route("{controller}")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Добавить комментарий
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("{action}")]
        public async Task<IActionResult> Add([FromForm] CommentCreateViewModel comment)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser();
                var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
                var newComment = _mapper.Map<Comment>(comment);
                newComment.AuthorId = currentUser.Id;
                await commentRepo.Create(newComment);
            }

            return RedirectToAction("GetById", "Article", new { id = comment.ArticleId });
        }

        /// <summary>
        /// Форма редактирования комментария
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var dbComment = await commentRepo.GetById(id);
            if (dbComment == null)
            {
                return NotFound($"Ошибка: Комментарий с идентификатором {id} не существует.");
            }

            var comment = _mapper.Map<CommentCreateViewModel>(dbComment);
            ViewData["Header"] = $"Статья: {dbComment.Article.Title}, Автор: {dbComment.Author.Name}";
            return View(comment);
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [HttpPost]
        [Route("{action}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromForm] CommentCreateViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                return View(comment);
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var oldComment = await commentRepo.GetById(comment.Id);
            if (oldComment == null)
            {
                return NotFound($"Ошибка: Комментарий с идентификатором {comment.Id} не существует.");
            }

            if (oldComment.AuthorId != currentUser.Id
                && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return NotFound($"Ошибка: Вы не имеете права редактировать этот комментарий.");

            var newComment = _mapper.Map(comment, oldComment);

            await commentRepo.Update(newComment);
            return RedirectToAction("GetById", "Article", new { id = comment.ArticleId });
        }

        /// <summary>
        /// Удалить комментарий
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var comment = await commentRepo.GetById(id);
            if (comment == null)
            {
                return NotFound($"Ошибка: Комментарий с идентификатором {id} не существует.");
            }

            if (comment.AuthorId != currentUser.Id
                && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return NotFound($"Ошибка: Вы не имеете права удалить этот комментарий.");

            await commentRepo.Delete(comment);

            return RedirectToAction("GetById", "Article", new { id = comment.ArticleId });
        }
    }
}