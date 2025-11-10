using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Common;

public static class ResultMapper
{
    public static IActionResult From<T>(Result<T> result)
    {
        var notifications = result.Notifications.Select(n => n.ToString()).ToArray();

        if (result.Success)
            return new OkObjectResult(new { data = result.Data, notifications = Array.Empty<string>() });

        if (notifications.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            return new NotFoundObjectResult(new { data = (object?)null, notifications });

        return new BadRequestObjectResult(new { data = (object?)null, notifications });
    }

    public static IActionResult From(Result result)
    {
        var notifications = result.Notifications.Select(n => n.ToString()).ToArray();

        if (result.Success)
            return new NoContentResult();

        return new BadRequestObjectResult(new { data = (object?)null, notifications });
    }
}