using Domain.Common;

namespace Presentation.Api.Common;

public class Result
{
    public IReadOnlyCollection<Notification> Notifications { get; }
    public bool Success => !Notifications.Any();

    protected Result(IEnumerable<Notification>? notifications)
    {
        Notifications = (notifications ?? Array.Empty<Notification>()).ToList().AsReadOnly();
    }

    public static Result Ok() => new Result(null);

    public static Result Fail(params Notification[] notifications) => new Result(notifications);

    public static Result Fail(string message) => new Result(new[] { new Notification(message) });

    public static Result Fail(string key, string message) => new Result(new[] { new Notification(key, message) });
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T? data, IEnumerable<Notification>? notifications) : base(notifications)
    {
        Data = data;
    }

    public static Result<T> Ok(T data) => new Result<T>(data, null);

    public static new Result<T> Fail(params Notification[] notifications) => new Result<T>(default, notifications);

    public static new Result<T> Fail(string message) => new Result<T>(default, new[] { new Notification(message) });

    public static new Result<T> Fail(string key, string message) => new Result<T>(default, new[] { new Notification(key, message) });
}