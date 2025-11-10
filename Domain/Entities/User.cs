using Domain.Validations;
using Domain.ValueObjects;
using FluentValidation.Results;

namespace Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        private readonly List<Tasks> _tasks = new();
        public IReadOnlyCollection<Tasks> Tasks => _tasks.AsReadOnly();

        // Parameterless constructor for EF Core
        protected User() { }

        // Public constructor used by application/domain
        public User(string? name, Email email)
        {
            Name = name?.Trim() ?? string.Empty;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }

        public static User Create(string? name, string emailAddress)
        {
            var email = new Email(emailAddress);
            return new User(name, email);
        }

        public ValidationResult EhValido()
            => new UserValidator().Validate(this);
    }
}
