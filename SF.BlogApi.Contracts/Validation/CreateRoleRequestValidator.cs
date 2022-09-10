using FluentValidation;

namespace SF.BlogApi.Contracts.Validation
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().
                WithMessage("Введите название роли");
        }
    }
}
