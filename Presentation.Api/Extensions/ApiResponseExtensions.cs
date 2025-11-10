using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Extensions;

public static class ApiResponseExtensions
{
    /// <summary>
    /// Verifica se há erros na resposta
    /// </summary>
    public static bool HasErrors<T>(this ApiResponse<T> response)
    {
        return response.Erros.Any();
    }

    /// <summary>
    /// Converte ApiResponse para IActionResult com status HTTP apropriado
    /// </summary>
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        return new OkObjectResult(response);
    }

    /// <summary>
    /// Converte ApiResponse para IActionResult com mapeamento automático (objeto único)
    /// Unifica Map + ToActionResult em uma única chamada
    /// </summary>
    public static IActionResult ToActionResult<TInput, TOutput>(
        this ApiResponse<TInput> response,
        Func<TInput, TOutput> mapper)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        if (response.Data is null)
        {
            var errorResponse = ApiResponse<TOutput>.Failure(
                new ApiError(500, "Dados inesperadamente nulos", "Data"));
            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }

        var mappedData = mapper(response.Data);
        var successResponse = ApiResponse<TOutput>.Success(mappedData);
        return new OkObjectResult(successResponse);
    }

    /// <summary>
    /// Converte ApiResponse de coleção para IActionResult com mapeamento automático
    /// Unifica MapCollection + ToActionResult em uma única chamada
    /// </summary>
    public static IActionResult ToActionResult<TInput, TOutput>(
        this ApiResponse<IEnumerable<TInput>> response,
        Func<TInput, TOutput> mapper)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        if (response.Data is null)
        {
            var errorResponse = ApiResponse<IEnumerable<TOutput>>.Failure(
                new ApiError(500, "Dados inesperadamente nulos", "Data"));
            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }

        var mappedData = response.Data.Select(mapper);
        var successResponse = ApiResponse<IEnumerable<TOutput>>.Success(mappedData);
        return new OkObjectResult(successResponse);
    }

    /// <summary>
    /// Converte ApiResponse para CreatedAtAction (201) se sucesso, ou erro apropriado.
    /// Aceita uma factory function para extrair route values do data.
    /// </summary>
    public static IActionResult ToCreatedAtActionResult<T>(
        this ApiResponse<T> response,
        string actionName,
        Func<T, object>? routeValueFactory = null)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        var routeValues = routeValueFactory?.Invoke(response.Data!);
        return new CreatedAtActionResult(actionName, null, routeValues, response);
    }

    /// <summary>
    /// Converte ApiResponse para CreatedAtAction com mapeamento automático
    /// Unifica Map + ToCreatedAtActionResult em uma única chamada
    /// </summary>
    public static IActionResult ToCreatedAtActionResult<TInput, TOutput>(
        this ApiResponse<TInput> response,
        string actionName,
        Func<TInput, TOutput> mapper,
        Func<TOutput, object>? routeValueFactory = null)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        if (response.Data is null)
        {
            var errorResponse = ApiResponse<TOutput>.Failure(
                new ApiError(500, "Dados inesperadamente nulos", "Data"));
            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }

        var mappedData = mapper(response.Data);
        var successResponse = ApiResponse<TOutput>.Success(mappedData);
        var routeValues = routeValueFactory?.Invoke(mappedData);
        return new CreatedAtActionResult(actionName, null, routeValues, successResponse);
    }

    /// <summary>
    /// Converte ApiResponse para NoContent (204) se sucesso, ou erro apropriado
    /// </summary>
    public static IActionResult ToNoContentResult<T>(this ApiResponse<T> response)
    {
        if (response.HasErrors())
        {
            var firstError = response.Erros.First();
            return new ObjectResult(response) { StatusCode = firstError.StatusCode };
        }

        return new NoContentResult();
    }

    /// <summary>
    /// Transforma o Data usando um mapper. Se houver erros, propaga os erros.
    /// Valida se Data não é nulo antes de aplicar o mapper.
    /// </summary>
    public static ApiResponse<TOutput> Map<TInput, TOutput>(
        this ApiResponse<TInput> response,
        Func<TInput, TOutput> mapper)
    {
        if (response.HasErrors())
            return ApiResponse<TOutput>.Failure(response.Erros.ToArray());

        if (response.Data is null)
            return ApiResponse<TOutput>.Failure(
                new ApiError(500, "Dados inesperadamente nulos", "Data"));

        return ApiResponse<TOutput>.Success(mapper(response.Data));
    }

    /// <summary>
    /// Transforma coleções usando um mapper. Se houver erros, propaga os erros.
    /// </summary>
    public static ApiResponse<IEnumerable<TOutput>> MapCollection<TInput, TOutput>(
        this ApiResponse<IEnumerable<TInput>> response,
        Func<TInput, TOutput> mapper)
    {
        if (response.HasErrors())
            return ApiResponse<IEnumerable<TOutput>>.Failure(response.Erros.ToArray());

        if (response.Data is null)
            return ApiResponse<IEnumerable<TOutput>>.Failure(
                new ApiError(500, "Dados inesperadamente nulos", "Data"));

        return ApiResponse<IEnumerable<TOutput>>.Success(response.Data.Select(mapper));
    }

    /// <summary>
    /// Versão assíncrona de Map para composição fluente com Tasks
    /// </summary>
    public static async Task<ApiResponse<TOutput>> MapAsync<TInput, TOutput>(
        this Task<ApiResponse<TInput>> responseTask,
        Func<TInput, TOutput> mapper)
    {
        var response = await responseTask;
        return response.Map(mapper);
    }

    /// <summary>
    /// Versão assíncrona de MapCollection para composição fluente com Tasks
    /// </summary>
    public static async Task<ApiResponse<IEnumerable<TOutput>>> MapCollectionAsync<TInput, TOutput>(
        this Task<ApiResponse<IEnumerable<TInput>>> responseTask,
        Func<TInput, TOutput> mapper)
    {
        var response = await responseTask;
        return response.MapCollection(mapper);
    }

    /// <summary>
    /// Converte Task de ApiResponse para IActionResult
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(
        this Task<ApiResponse<T>> responseTask)
    {
        var response = await responseTask;
        return response.ToActionResult();
    }

    /// <summary>
    /// Converte Task de ApiResponse para CreatedAtActionResult
    /// </summary>
    public static async Task<IActionResult> ToCreatedAtActionResultAsync<T>(
        this Task<ApiResponse<T>> responseTask,
        string actionName,
        Func<T, object>? routeValueFactory = null)
    {
        var response = await responseTask;
        return response.ToCreatedAtActionResult(actionName, routeValueFactory);
    }

    /// <summary>
    /// Converte Task de ApiResponse para NoContentResult
    /// </summary>
    public static async Task<IActionResult> ToNoContentResultAsync<T>(
        this Task<ApiResponse<T>> responseTask)
    {
        var response = await responseTask;
        return response.ToNoContentResult();
    }
}

