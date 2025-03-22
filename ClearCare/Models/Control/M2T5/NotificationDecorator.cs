using ClearCare.Interfaces;
using System.Threading.Tasks;

public abstract class NotificationDecorator : INotificationSender
{
    protected INotificationSender _notificationSender;

    public NotificationDecorator(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public virtual async Task sendNotification(string email, string phone, string content)
    {
        await _notificationSender.sendNotification(email, phone, content);
    }
}
