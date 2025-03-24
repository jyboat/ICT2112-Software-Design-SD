using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface INotificationSender
    {
        Task sendNotification(string email, string phone, string content);
    }
}