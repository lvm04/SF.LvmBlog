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
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        /// <summary>
        /// Просмотр списка тегов
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tags = await tagRepo.GetAll();
            var tagResponse = new
            {
                TagsAmount = tags.Count(),
                Tags = tags.ToArray()
            };

            return StatusCode(200, tagResponse);
        }

        /// <summary>
        /// Получить тег по ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.Get(id);
            if (tag == null)
                return StatusCode(400, $"Ошибка: Тег с идентификатором {id} не существует.");

            return StatusCode(200, tag);  
        }

        /// <summary>
        /// Создание тега
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Create(string tagName)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.GetByName(tagName);

            if (tag != null)
                return StatusCode(400, $"Ошибка: Тег с таким названием уже существует.");

            await tagRepo.Create(new Tag { Name = tagName });

            return StatusCode(200, $"Тег \"{tagName}\" создан");
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]string tagName)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.Get(id);
            if (tag == null)
                return StatusCode(400, $"Ошибка: Тег с идентификатором {id} не существует.");

            tag.Name = tagName;
            await tagRepo.Update(tag);
            return StatusCode(200, tag);
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var tag = await tagRepo.Get(id);
            if (tag == null)
                return StatusCode(400, $"Ошибка: Тег с идентификатором {id} не существует.");

            await tagRepo.Delete(tag);

            return StatusCode(200, $"Тег \"{tag.Name}\" удален");
        }



    }
}
