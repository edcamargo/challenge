using Domain.Entities;
using FluentValidation;

namespace Domain.Validations;

public class TaskValidator : AbstractValidator<Tasks>
{
    public TaskValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("O título da tarefa é obrigatório.")
            .Length(2, 200).WithMessage("O título da tarefa deve ter entre 2 e 200 caracteres.");
        RuleFor(t => t.Description)
            .MaximumLength(1000).WithMessage("A descrição da tarefa não pode exceder 1000 caracteres.");
        RuleFor(t => t.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A data de vencimento deve ser no futuro.")
            .When(t => t.DueDate.HasValue);
        RuleFor(t => t.UserId)
            .NotEmpty().WithMessage("O ID do usuário associado à tarefa é obrigatório."); 
    }
}