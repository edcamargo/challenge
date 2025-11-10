using Domain.ValueObjects;
using FluentValidation;

namespace Domain.Validations;

internal class EmailValidator : AbstractValidator<Email>
{
    public EmailValidator()
    {
        RuleFor(e => e.Endereco)
            .Must(Email.IsValid).WithMessage("O e-mail do cliente está em formato inválido.");
    }
}