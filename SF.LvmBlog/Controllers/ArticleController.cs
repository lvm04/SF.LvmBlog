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
        public async Task<IActionResult> GetById([FromRoute] int id)
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
            var newComment = new Comment { AuthorId = currentUser.Id, ArticleId = id, Text = text };
            await commentRepo.Create(newComment);

            return RedirectToAction("GetById", "Article", new { id });
        }


        /// <summary>
        /// Форма создания статьи
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("New")]
        public async Task<IActionResult> Create()
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var _tags = await tagRepo.GetAll();

            var article = new ArticleCreateViewModel();
            //article.Id = -1;
            article.TagNames = _tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name, isChecked = false }).ToArray();

            ViewData["Title"] = "Новая статья";
            ViewData["Header"] = "Добавление статьи";
            ViewData["Method"] = "Create";

            return View(article);
        }

        /// <summary>
        /// Создание статьи
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create(ArticleCreateViewModel article)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;

            var dbArticle = _mapper.Map<Article>(article);
            dbArticle.AuthorId = currentUser.Id;
            dbArticle.Tags = await GetRepoTags(tagRepo, article.Tags);

            await articleRepo.Create(dbArticle);
            return RedirectToAction("Index", "Article");
        }

        // Преобразование списка тегов
        private async Task<List<Tag>> GetRepoTags(TagRepository repo, string[] tags)
        {
            var tagList = new List<Tag>();
            if (tags == null || tags.Length == 0)
                return tagList;

            foreach (var tagName in tags)
            {
                var dbTag = await repo.GetByName(tagName);
                if (dbTag != null)
                    tagList.Add(dbTag);
            }
            return tagList;
        }

        /// <summary>
        /// Форма редактирования статьи
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("{action}/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.GetById(id);
            if (oldArticle == null)
            {
                return NotFound($"Ошибка: Статья с идентификатором {id} не существует.");
            }

            if (oldArticle.AuthorId != currentUser.Id
                 && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return NotFound($"Ошибка: Вы не имеете права редактировать эту статью.");

            var article = _mapper.Map<ArticleCreateViewModel>(oldArticle);

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var _tags = await tagRepo.GetAll();

            article.TagNames = _tags.Select(t =>
                new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    isChecked = oldArticle.Tags.Select(t1 => t1.Name).Contains(t.Name)
                }).ToArray();

            ViewData["Title"] = "Редактирование статьи";
            ViewData["Header"] = "Редактирование статьи";
            ViewData["Method"] = "Edit";

            return View("Create", article);
        }

        /// <summary>
        /// Редактирование статьи
        /// </summary>
        [HttpPost]
        [Route("{action}/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ArticleCreateViewModel article)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.GetById(id);
            if (oldArticle == null)
            {
                return NotFound($"Ошибка: Статья с идентификатором {id} не существует.");
            }

            if (oldArticle.AuthorId != currentUser.Id
                && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return NotFound($"Ошибка: Вы не имеете права редактировать эту статью.");

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var newArticle = _mapper.Map(article, oldArticle);

            newArticle.Tags = await GetRepoTags(tagRepo, article.Tags);

            await articleRepo.Update(newArticle);
            return RedirectToAction("Index", "Article");
        }

        /// <summary>
        /// Удаление статьи
        /// </summary>
        [HttpGet]
        [Route("{action}/{id}")]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var article = await articleRepo.Get(id);
            if (article == null)
                return NotFound($"Ошибка: Статья с идентификатором {id} не существует.");

            await articleRepo.Delete(article);

            return RedirectToAction("Index", "Article");
        }
    }
}
