using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SF.BlogApi.Contracts;
using SF.BlogData;
using SF.BlogData.Models;
using SF.BlogData.Repository;
using System.Data;
using System.Security.Claims;

namespace SF.BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTagRequest> _validator;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateTagRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
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
        [Route("Add")]
        [Authorize]
        public async Task<IActionResult> Create(CreateTagRequest tag)
        {
            var validateResult = await _validator.ValidateAsync(tag);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var _tag = await tagRepo.GetByName(tag.Name);

            if (_tag != null)
                return StatusCode(400, $"Ошибка: Тег с таким названием уже существует.");

            await tagRepo.Create(new Tag { Name = tag.Name });

            return StatusCode(200, $"Тег \"{tag.Name}\" создан");
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [HttpPut]
        [Route("[action]/{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody] CreateTagRequest tag)
        {
            var validateResult = await _validator.ValidateAsync(tag);
            if (!validateResult.IsValid)
            {
                return StatusCode(400, Results.ValidationProblem(validateResult.ToDictionary()));
            }

            var tagRepo = _unitOfWork.GetRepository<Tag>() as TagRepository;
            var _tag = await tagRepo.Get(id);
            if (_tag == null)
                return StatusCode(400, $"Ошибка: Тег с идентификатором {id} не существует.");

            _tag.Name = tag.Name;
            await tagRepo.Update(_tag);
            return StatusCode(200, _tag);
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        [HttpDelete]
        [Route("[action]/{id}")]
        [Authorize(Roles = "Администратор")]
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
