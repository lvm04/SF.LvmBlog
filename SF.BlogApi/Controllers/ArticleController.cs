using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SF.BlogApi.Contracts;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using System.Data;

namespace SF.BlogApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateArticleRequest> _validator;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateArticleRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
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
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");
            var article = _mapper.Map<ArticleView>(_article);

            return StatusCode(200, article);
        }

        /// <summary>
        /// Просмотр списка статей
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            var _articles = await articleRepo.GetAllWithTags();
            var articles = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleView>>(_articles);
            var articleResponse = new GetArticlesResponse
            {
                ArticlesAmount = articles.Count(),
                Articles = articles.ToArray()
            };

            return StatusCode(200, articleResponse);
        }

        /// <summary>
        /// Создание статьи
        /// </summary>
        [HttpPost]
        [Route("Add")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateArticleRequest article)
        {
            var validateResult = await _validator.ValidateAsync(article);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var dbArticle = _mapper.Map<Article>(article);
            var currentUser = await HttpContext.GetCurrentUser();
            dbArticle.AuthorId = currentUser.Id;
            await articleRepo.Create(dbArticle);

            return StatusCode(200, $"Статья \"{dbArticle.Title}\" добавлена с ID {dbArticle.Id}");
        }

        /// <summary>
        /// Редактирование статьи
        /// </summary>
        [HttpPut]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CreateArticleRequest article)
        {
            var validateResult = await _validator.ValidateAsync(article);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

            var currentUser = await HttpContext.GetCurrentUser();

            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.Get(id);

            if (oldArticle == null)
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");

            // Проверяем может ли текущий пользователь редактировать данную статью
            // Для этого он должен быть автором или принадлежать к группе admin или moderator
            if (oldArticle.AuthorId != currentUser.Id
                && currentUser.Roles.FirstOrDefault(r => r.Name == "Администратор") == null
                && currentUser.Roles.FirstOrDefault(r => r.Name == "Модератор") == null)
                return StatusCode(400, $"Ошибка: Вы не имеете право редактировать эту статью.");

            var newArticle = _mapper.Map(article, oldArticle);
            await articleRepo.Update(newArticle);

            return StatusCode(200, _mapper.Map<ArticleView>(newArticle));
        }

        /// <summary>
        /// Редактирование тегов статьи
        /// </summary>
        [HttpPut]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditTags([FromRoute] int id, [FromBody] string[] tags, [FromServices] ApplicationDbContext db)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            var article = await db.Articles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
                return StatusCode(400, $"Ошибка: Статья с идентификатором {id} не существует.");

            // Проверяем может ли текущий пользователь редактировать данную статью
            // Для этого он должен быть автором или принадлежать к группе admin или moderator
            if (article.AuthorId != currentUser.Id
                && currentUser.Roles.FirstOrDefault(r => r.Name == "Администратор") == null
                && currentUser.Roles.FirstOrDefault(r => r.Name == "Модератор") == null)
                return StatusCode(400, $"Ошибка: Вы не имеете право редактировать эту статью.");

            var tagList = new List<Tag>();
            foreach (var tag in tags)
            {
                var dbTag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tag);
                if (dbTag != null)
                    tagList.Add(dbTag);
            }

            article.Tags = tagList;
            //db.Log = Console.WriteLine;
            db.SaveChanges();
            return StatusCode(200, _mapper.Map<ArticleView>(article));
        }

        /// <summary>
        /// Удаление статьи
        /// </summary>
        [HttpDelete]
        [Route("[action]/{id}")]
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



    }
}
