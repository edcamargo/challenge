using Domain.Entities;
using FluentValidation;

namespace Domain.Validations;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
            .Length(2, 100).WithMessage("O nome do usuário deve ter entre 2 e 100 caracteres.");

        RuleFor(c => c.Email)
            .NotNull().WithMessage("O e-mail do usuário está em formato inválido.")
            .SetValidator(new EmailValidator());
    }
}