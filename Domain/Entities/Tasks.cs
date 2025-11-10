using Domain.Common;
using FluentValidation.Results;

namespace Domain.Entities
{
    public class Tasks : Notifiable
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public bool IsCompleted { get; private set; }

        // Parameterless constructor for EF Core
        private Tasks() { }

        // Protected constructor used by factory
        protected Tasks(string? title, string? description, DateTime? dueDate, User? user)
        {
            Id = Guid.NewGuid();
            Title = title?.Trim() ?? string.Empty;
            Description = description?.Trim();
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            UserId = user?.Id ?? Guid.Empty;
            User = user;
            IsCompleted = false;
        }

        public static Tasks Create(string? title, string? description, DateTime? dueDate, User? user)
        {
            var task = new Tasks(title, description, dueDate, user);

            if (string.IsNullOrWhiteSpace(task.Title))
                task.AddNotification(nameof(Title), "Title is required.");

            if (user == null)
                task.AddNotification(nameof(User), "User is required.");

            if (dueDate.HasValue && dueDate.Value < task.CreatedAt)
                task.AddNotification(nameof(DueDate), "DueDate cannot be in the past.");

            // If user exists, validate it via FluentValidation and add its errors as Notifications
            if (user != null)
            {
                ValidationResult validation = user.EhValido();
                if (!validation.IsValid)
                {
                    foreach (var failure in validation.Errors)
                    {
                        task.AddNotification(failure.PropertyName, failure.ErrorMessage);
                    }
                }
            }

            return task;
        }

        public void MarkCompleted() => IsCompleted = true;
        public void MarkPending() => IsCompleted = false;

        public void UpdateTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                AddNotification(nameof(Title), "Title is required.");
                return;
            }

            Title = title.Trim();
        }

        public void UpdateDescription(string? description)
        {
            Description = description?.Trim();
        }

        public void UpdateDueDate(DateTime? dueDate)
        {
            if (dueDate.HasValue && dueDate.Value < CreatedAt)
            {
                AddNotification(nameof(DueDate), "DueDate cannot be before CreatedAt.");
                return;
            }

            DueDate = dueDate;
        }
    }
}
