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

namespace SF.LvmBlog.Controllers
{

    [Route("{controller}")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Список всех статей
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            var _articles = await articleRepo.GetAllWithTags();
            var articles = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleShortViewModel>>(_articles);

            return View(articles);
        }

        
        /// <summary>
        /// Получить статью по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var _article = await articleRepo.GetById(id);
            if (_article == null)
            {
                return NotFound($"Ошибка: Статья с идентификатором {id} не существует.");
            }
            var article = _mapper.Map<ArticleViewModel>(_article);

            _article.NumberViews++;
            await articleRepo.Update(_article);
            
            return View(article);   
        }

        /// <summary>
        /// Добавить комментарий
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, string text)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var commentRepo = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var newComment = new Comment { AuthorId = currentUser.Id, ArticleId = id, Text = text};
            await commentRepo.Create(newComment);

            return RedirectToAction("GetById", "Article", new { id });
        }


        /// <summary>
        /// Создание статьи
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("New")]
        public async Task<IActionResult> Create()
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var _tags = await tagRepo.GetAll();

            var article = new ArticleCreateViewModel();
            article.Tags = _tags.Select(t => false).ToArray();
            article.TagNames = _tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name, isChecked = false }).ToArray();

            //dbArticle.AuthorId = currentUser.Id;
            //await articleRepo.Create(dbArticle);


            return View(article);
        }

        
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create(ArticleCreateViewModel article)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            
            //var dbArticle = _mapper.Map<Article>(article);
            
            //dbArticle.AuthorId = currentUser.Id;
            //await articleRepo.Create(dbArticle);

            return RedirectToAction("GetById", "Article");

        }

      /*
        /// <summary>
        /// Редактирование статьи
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]CreateArticleRequest article)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.Get(id);

            if (oldArticle == null)
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");

            // Проверяем может ли текущий пользователь редактировать данную статью
            // Для этого он должен быть автором или принадлежать к группе admin или moderator
            if (oldArticle.AuthorId != currentUser.Id
                && currentUser.Roles.FirstOrDefault(r => r.Name == "admin") == null
                && currentUser.Roles.FirstOrDefault(r => r.Name == "moderator") == null)
                return StatusCode(400, $"Ошибка: Вы не имеете право редактировать эту статью.");

            var newArticle = _mapper.Map(article, oldArticle);
            await articleRepo.Update(newArticle);

            return StatusCode(200, _mapper.Map<ArticleView>(newArticle));
        }

        /// <summary>
        /// Удаление статьи
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var article = await articleRepo.Get(id);
            if (article == null)
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");

            await articleRepo.Delete(article);

            return StatusCode(200, $"Статья с ID {id} удалена");
        }

    */

    }
}
