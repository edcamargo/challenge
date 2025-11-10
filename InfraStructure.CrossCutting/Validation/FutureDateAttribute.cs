using System;
using System.ComponentModel.DataAnnotations;

namespace InfraStructure.CrossCutting.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; // optional
            if (value is DateTime dt)
            {
                if (dt >= DateTime.UtcNow.Date) return ValidationResult.Success;
                return new ValidationResult(ErrorMessage ?? "The date must be today or in the future.");
            }

            return new ValidationResult(ErrorMessage ?? "Invalid date value.");
        }
    }
}

