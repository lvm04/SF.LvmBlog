using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.BlogApi.Contracts.Validation
{
    public class EditCommentRequestValidator : AbstractValidator<EditCommentRequest>
    {
        public EditCommentRequestValidator()
        {
            RuleFor(x => x.Text).NotEmpty().
                WithMessage("Введите текст");
        }
    }
}
