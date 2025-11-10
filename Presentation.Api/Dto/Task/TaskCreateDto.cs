using System;
using System.ComponentModel.DataAnnotations;
using InfraStructure.CrossCutting.Validation;

namespace Presentation.Api.Dto
{
    public class TaskCreateDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title can be at most 200 characters.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [FutureDate(ErrorMessage = "DueDate cannot be in the past.")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [NotEmptyGuid(ErrorMessage = "UserId must be a valid GUID.")]
        public Guid UserId { get; set; }
    }
}
