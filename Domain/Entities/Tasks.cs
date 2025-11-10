using Domain.Validations;
using FluentValidation.Results;

namespace Domain.Entities
{
    public class Tasks : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public bool IsCompleted { get; private set; }

        // Parameterless constructor for EF Core
        protected Tasks() { }

        // Protected constructor used by factory
        public Tasks(string? title, string? description, DateTime? dueDate, Guid userId, User? user = null)
        {
            Title = title?.Trim() ?? string.Empty;
            Description = description?.Trim();
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            UserId = userId;
            User = user;
            IsCompleted = false;
        }
        public static Tasks Create(string? title, string? description, DateTime? dueDate, Guid userId)
        {
            var task = new Tasks(title, description, dueDate, userId);
            return task;
        }
        public void MarkCompleted() => IsCompleted = true;
        public void MarkPending() => IsCompleted = false;
        public ValidationResult EhValido()    
            => new TaskValidator().Validate(this);
    }
}
