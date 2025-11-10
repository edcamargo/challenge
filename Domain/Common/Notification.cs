using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

[NotMapped]
public sealed class Notification
{
    public string Key { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }

    public Notification(string key, string message)
    {
        Key = key ?? string.Empty;
        Message = message ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public Notification(string message) : this(string.Empty, message) { }

    public override string ToString() => $"{Key}: {Message}";
}