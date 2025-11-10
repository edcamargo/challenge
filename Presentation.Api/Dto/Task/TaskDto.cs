using System;

namespace Presentation.Api.Dto
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid UserId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
