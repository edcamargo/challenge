namespace Domain.Validations;

public class OperationResult<T>
{
    public object Data { get; set; }
    public FluentValidation.Results.ValidationResult ValidationResult { get; set; }

    public bool Success => ValidationResult?.IsValid == true;

    public OperationResult(object data, FluentValidation.Results.ValidationResult validationResult)
    {
        Data = data;
        ValidationResult = validationResult;
    }
}