using FluentValidation;

namespace SF.BlogApi.Contracts.Validation
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty()
                .Must(t => t.Trim().Length >= 3 && t.Trim().Length <= 20)
                .WithMessage("Логин должен иметь длину от 3 до 20 символов");

            RuleFor(x => x.Password).NotEmpty()
                .Must(t => t.Trim().Length >= 3 && t.Trim().Length <= 20)
                .WithMessage("Пароль должен иметь длину от 3 до 20 символов");

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Введите имя");

            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress()
                .WithMessage("Почтовый адрес не корректен");
        }
    }
}
