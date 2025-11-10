using FluentValidation.Results;

namespace Application.Custom;

public class ActionErrorCustom
{
    public static void ActionError(
        ValidationResult result,
        string property,
        string message) => result.Errors.Add(new ValidationFailure(property, message));
}