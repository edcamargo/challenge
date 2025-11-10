using System;
using System.ComponentModel.DataAnnotations;

namespace InfraStructure.CrossCutting.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NotEmptyGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is Guid guid && guid != Guid.Empty) return ValidationResult.Success;
            return new ValidationResult(ErrorMessage ?? "GUID is required.");
        }
    }
}

