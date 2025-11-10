namespace Domain.Validations;

public class OperationResult<T>
{
    public T Data { get; set; }
    public FluentValidation.Results.ValidationResult ValidationResult { get; set; }

    public bool Success => ValidationResult?.IsValid == true;

    public OperationResult(T data, FluentValidation.Results.ValidationResult validationResult)
    {
        Data = data;
        ValidationResult = validationResult;
    }
}