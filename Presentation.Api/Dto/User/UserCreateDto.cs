using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Api.Dto
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can be at most 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
        [StringLength(200, ErrorMessage = "Email can be at most 200 characters.")]
        public string Email { get; set; } = string.Empty;
    }
}
