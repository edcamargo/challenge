using FluentValidation.Results;

namespace Application.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public List<ApiError> Erros { get; set; } = new();

    public static ApiResponse<T> Success(T data) => new()
    {
        Data = data,
        Erros = new List<ApiError>()
    };

    public static ApiResponse<T> Failure(params ApiError[] errors) => new()
    {
        Data = default,
        Erros = errors.ToList()
    };

    /// <summary>
    /// Cria uma resposta de falha a partir de um ValidationResult do FluentValidation
    /// </summary>
    public static ApiResponse<T> ValidationFailure(ValidationResult validationResult, int statusCode = 400)
    {
        var errors = validationResult.Errors
            .Select(e => new ApiError(statusCode, e.ErrorMessage, e.PropertyName))
            .ToArray();
        return Failure(errors);
    }

    /// <summary>
    /// Cria uma resposta de NotFound (404) com mensagem customizada
    /// </summary>
    public static ApiResponse<T> NotFound(string message = "Recurso não encontrado", string key = "id")
    {
        return Failure(new ApiError(404, message, key));
    }

    /// <summary>
    /// Cria uma resposta de erro com status code e mensagem customizados
    /// </summary>
    public static ApiResponse<T> Error(int statusCode, string message, string key = "")
    {
        return Failure(new ApiError(statusCode, message, key));
    }
}

public class ApiError
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Key { get; set; }

    public ApiError(int statusCode, string message, string? key = null)
    {
        StatusCode = statusCode;
        Message = message;
        Key = key;
    }
}

