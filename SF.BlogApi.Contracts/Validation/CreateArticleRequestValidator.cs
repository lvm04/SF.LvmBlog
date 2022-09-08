using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.BlogApi.Contracts.Validation
{
    public class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
    {
        public CreateArticleRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty()
                .Must(t => t.Trim().Length >= 3 && t.Trim().Length <= 250)
                .WithMessage("Заголовок статьи должен иметь длину от 3 до 250 символов");
            RuleFor(x => x.Text).NotEmpty().
                WithMessage("Введите текст");
        }
    }
}
