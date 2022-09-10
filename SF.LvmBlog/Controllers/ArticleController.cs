using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using SF.LvmBlog.ViewModels;
using System.Data;

namespace SF.LvmBlog.Controllers
{

    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArticleController> _logger;
        private IMapper _mapper, _mapperMod;
        const int PAGE_SIZE = 5;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ArticleController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Модификация маппера для обрезания текста статьи
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Article, ArticleViewModel>()
                    .ForMember(dest => dest.Text, opt => opt.MapFrom(src =>
                        src.Text.Length > 100 ? src.Text.Substring(0, 100) : src.Text.Substring(0, src.Text.Length)));
            });
            _mapperMod = configuration.CreateMapper();
            _logger = logger;
        }

        /// <summary>
        /// Список статей
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int page = 1, SortState sortOrder = SortState.Id, bool up = true,
                                                string searchText = "", string tagName = "")
        {
            //int a = 1, b = 0; int c = a / b;      // исключение

            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            IQueryable<Article> _articles = articleRepo.Set.Include(a => a.Author).Include(a => a.Tags).AsNoTracking();

            // Фильтрация
            if (!string.IsNullOrWhiteSpace(searchText))
                _articles = _articles.Where(a => EF.Functions.Like(a.Text, $"%{searchText}%") ||
                                                 EF.Functions.Like(a.Title, $"%{searchText}%"));

            if (!string.IsNullOrWhiteSpace(tagName))
                _articles = _articles.Where(a => a.Tags.Select(t => t.Name).Contains(tagName));

            // Сортировка
            _articles = sortOrder switch
            {
                SortState.Rating => up ? _articles.OrderBy(d => d.NumberViews) : _articles.OrderByDescending(d => d.NumberViews),
                SortState.CreateDate => up ? _articles.OrderBy(d => d.TimeStamp) : _articles.OrderByDescending(d => d.TimeStamp),
                _ => _articles.OrderBy(d => d.Id)
            };

            // Пагинация
            var count = await _articles.CountAsync();
            var items = await _articles.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToListAsync();

            var articles = _mapperMod.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(items);

            var articlesModel = new ArticleListViewModel()
            {
                Articles = articles,
                PageViewModel = new PageViewModel(count, page, PAGE_SIZE),
                SortViewModel = new SortViewModel(sortOrder, up),
                SearchText = searchText,
                TagName = tagName
            };

            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил список статей");

            ViewData["SearchText"] = searchText;
            ViewData["TagName"] = tagName;
            ViewData["Header"] = "Статьи";
            return View(articlesModel);
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
                return View("UserError", $"Ошибка: Статья с идентификатором {id} не существует.");
            }
            var article = _mapper.Map<ArticleViewModel>(_article);

            _article.NumberViews++;
            await articleRepo.Update(_article);

            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил статью ID={id}");
            return View(article);
        }

        /// <summary>
        /// Поиск статей (не исп.)
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Search([FromQuery] string searchText)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            IEnumerable<Article> _articles;
            if (string.IsNullOrEmpty(searchText))
            {
                ViewData["Header"] = $"Все статьи";
                _articles = await articleRepo.GetAllWithTags();
            }
            else
            {
                ViewData["Header"] = $"Статьи по фильтру \"{searchText}\"";
                _articles = await articleRepo.GetByText(searchText);
            }

            var articles = _mapperMod.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(_articles);

            var articlesModel = new ArticleListViewModel()
            {
                Articles = articles,
                PageViewModel = new PageViewModel(articles.Count(), 1, PAGE_SIZE),
                SortViewModel = new SortViewModel(SortState.Id, true)
            };
            return View("Index", articlesModel);
        }

        /// <summary>
        /// Статьи по тегу (не исп.)
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetByTag([FromQuery] string tagName)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var _articles = await articleRepo.GetByTag(tagName);

            ViewData["Header"] = $"Статьи по тегу \"{tagName}\"";
            var articles = _mapperMod.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(_articles);

            var articlesModel = new ArticleListViewModel()
            {
                Articles = articles,
                PageViewModel = new PageViewModel(articles.Count(), 1, PAGE_SIZE),
                SortViewModel = new SortViewModel(SortState.Id, true)
            };

            return View("Index", articlesModel);
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
            article.Tags = _tags.Select(t => new OptionViewModel { Id = t.Id, Name = t.Name, isChecked = false }).ToArray();

            ViewSettings(true);

            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил форму создания статьи");
            return View(article);
        }

        private void ViewSettings(bool isCreate)
        {
            if (isCreate)
            {
                ViewData["Title"] = "Новая статья";
                ViewData["Header"] = "Добавление статьи";
                ViewData["Method"] = "Create";
            }
            else
            {
                ViewData["Title"] = "Редактирование статьи";
                ViewData["Header"] = "Редактирование статьи";
                ViewData["Method"] = "Edit";
            }
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

            if (!ModelState.IsValid)
            {
                ViewSettings(true);
                article.Tags = await GetViewTags(tagRepo, article.OptionNames);
                return View(article);
            }

            var dbArticle = _mapper.Map<Article>(article);
            dbArticle.AuthorId = currentUser.Id;
            dbArticle.Tags = await GetRepoTags(tagRepo, article.OptionNames);

            await articleRepo.Create(dbArticle);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] написал статью с Id={dbArticle.Id}");
            return RedirectToAction("Index", "Article");
        }

        /// <summary>
        /// Преобразование списка имен тегов в список Tag для БД
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="tags"></param>
        /// <returns>Task<List<Tag>></returns>
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
        /// Преобразование списка имен тегов в список OptionViewModel для представления
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="tags"></param>
        /// <returns>Task<OptionViewModel[]></returns>
        private async Task<OptionViewModel[]> GetViewTags(TagRepository repo, string[] tagNames)
        {
            var _tags = await repo.GetAll();
            var tagOptions = _tags.Select(t =>
            new OptionViewModel
            {
                Id = t.Id,
                Name = t.Name,
                isChecked = tagNames.Contains(t.Name)
            }).ToArray();

            return tagOptions;
        }

        /// <summary>
        /// Форма редактирования статьи
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.GetById(id);
            if (oldArticle == null)
            {
                return View("UserError", $"Ошибка: Статья с идентификатором {id} не существует.");
            }

            if (oldArticle.AuthorId != currentUser.Id
                 && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return View("UserError", $"Ошибка: Вы не имеете права редактировать эту статью.");

            var article = _mapper.Map<ArticleCreateViewModel>(oldArticle);
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;

            article.Tags = await GetViewTags(tagRepo, oldArticle.Tags.Select(t => t.Name).ToArray());
            ViewSettings(false);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] запросил для редактированя статью с Id={id}");
            return View("Create", article);
        }

        /// <summary>
        /// Редактирование статьи
        /// </summary>
        [HttpPost]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ArticleCreateViewModel article)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            if (!ModelState.IsValid)
            {
                ViewSettings(false);
                article.Tags = await GetViewTags(tagRepo, article.OptionNames);
                return View("Create", article);
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var oldArticle = await articleRepo.GetById(id);
            if (oldArticle == null)
            {
                return View("UserError", $"Ошибка: Статья с идентификатором {id} не существует.");
            }

            if (oldArticle.AuthorId != currentUser.Id
                && !currentUser.Roles.Any(r => r.Value == Roles.Admin || r.Value == Roles.Moderator))
                return View("UserError", $"Ошибка: Вы не имеете права редактировать эту статью.");

            var newArticle = _mapper.Map(article, oldArticle);
            newArticle.Tags = await GetRepoTags(tagRepo, article.OptionNames);

            await articleRepo.Update(newArticle);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] отредактировал статью с Id={id}");
            return RedirectToAction("Index", "Article");
        }

        /// <summary>
        /// Удаление статьи
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        [AuthorizeRoles(Roles.Admin, Roles.Moderator)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            var article = await articleRepo.Get(id);
            if (article == null)
                return View("UserError", $"Ошибка: Статья с идентификатором {id} не существует.");

            await articleRepo.Delete(article);
            _logger.LogInformation($"[{HttpContext.User.Identity.Name}] удалил статью с Id={id}");
            return RedirectToAction("Index", "Article");
        }

        /// <summary>
        /// Генерация статей
        /// </summary>
        [HttpGet]
        [Route("[action]/{amount}")]
        public async Task<IActionResult> Generate([FromRoute] int amount)
        {
            var articleRepo = _unitOfWork.GetRepository<Article>() as ArticleRepository;
            await articleRepo.Generate(amount);

            return RedirectToAction("Index");
        }
    }
}
