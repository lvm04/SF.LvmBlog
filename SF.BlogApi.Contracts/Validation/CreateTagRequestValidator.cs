using FluentValidation;

namespace SF.BlogApi.Contracts.Validation
{
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().
                WithMessage("Введите название тега");
        }
    }
}
