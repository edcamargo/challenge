namespace Domain.Common;

public abstract class Notifiable
{
    private readonly List<Notification> _notifications = new();

    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    public bool IsValid => !_notifications.Any();

    protected void AddNotification(string key, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        _notifications.Add(new Notification(key, message));
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification == null) return;
        _notifications.Add(notification);
    }

    protected void AddNotifications(IEnumerable<Notification>? notifications)
    {
        if (notifications == null) return;
        _notifications.AddRange(notifications.Where(n => n != null));
    }

    protected void AddNotifications(Notifiable? other)
    {
        if (other == null) return;
        AddNotifications(other.Notifications);
    }

    protected void ClearNotifications() => _notifications.Clear();
}