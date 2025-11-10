using Domain.Entities;
using FluentAssertions;

namespace Challenge.Test.Unit.Domain.Entities
{
    public class TasksTests
    {
        [Fact]
        public void Create_WithValidData_IsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var due = DateTime.UtcNow.AddDays(2);

            // Act
            var task = Tasks.Create("Valid Title", "A valid description", due, userId);
            var result = task.EhValido();

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_WithEmptyTitle_IsInvalid()
        {
            var userId = Guid.NewGuid();
            var task = Tasks.Create("", "desc", DateTime.UtcNow.AddDays(1), userId);

            var result = task.EhValido();

            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.ErrorMessage.Contains("O título da tarefa é obrigatório")).Should().BeTrue();
        }

        [Fact]
        public void Create_WithShortTitle_IsInvalid()
        {
            var userId = Guid.NewGuid();
            var task = Tasks.Create("A", "desc", DateTime.UtcNow.AddDays(1), userId);

            var result = task.EhValido();

            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.ErrorMessage.Contains("deve ter entre 2 e 200 caracteres")).Should().BeTrue();
        }

        [Fact]
        public void Create_WithLongDescription_IsInvalid()
        {
            var userId = Guid.NewGuid();
            var longDesc = new string('x', 1001);
            var task = Tasks.Create("Title", longDesc, DateTime.UtcNow.AddDays(1), userId);

            var result = task.EhValido();

            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.ErrorMessage.Contains("A descrição da tarefa não pode exceder 1000 caracteres")).Should().BeTrue();
        }

        [Fact]
        public void Create_WithPastDueDate_IsInvalid()
        {
            var userId = Guid.NewGuid();
            var past = DateTime.UtcNow.AddDays(-1);
            var task = Tasks.Create("Title", "desc", past, userId);

            var result = task.EhValido();

            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.ErrorMessage.Contains("A data de vencimento deve ser no futuro")).Should().BeTrue();
        }

        [Fact]
        public void Create_WithEmptyUserId_IsInvalid()
        {
            var task = Tasks.Create("Title", "desc", DateTime.UtcNow.AddDays(1), Guid.Empty);

            var result = task.EhValido();

            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.ErrorMessage.Contains("O ID do usuário associado à tarefa é obrigatório")).Should().BeTrue();
        }

        [Fact]
        public void CreatedAt_IsSet_WhenCreated()
        {
            var userId = Guid.NewGuid();
            var before = DateTime.UtcNow;
            var task = Tasks.Create("Title", "desc", null, userId);
            var after = DateTime.UtcNow;

            task.CreatedAt.Should().BeOnOrAfter(before);
            task.CreatedAt.Should().BeOnOrBefore(after);
        }

        [Fact]
        public void MarkCompleted_And_MarkPending_Work()
        {
            var userId = Guid.NewGuid();
            var task = Tasks.Create("Title", "desc", null, userId);

            task.IsCompleted.Should().BeFalse();

            task.MarkCompleted();
            task.IsCompleted.Should().BeTrue();

            task.MarkPending();
            task.IsCompleted.Should().BeFalse();
        }
    }
}

