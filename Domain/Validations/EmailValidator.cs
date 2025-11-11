using Domain.ValueObjects;
using FluentValidation;

namespace Domain.Validations;

internal class EmailValidator : AbstractValidator<Email>
{
    public EmailValidator()
    {
        RuleFor(e => e.Endereco)
            .Must(Email.IsValid).WithMessage("O e-mail do usuário está em formato inválido.");
    }
}