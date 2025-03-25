using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface INotification
    {
        // Creates a notification for the given user with the provided content.
        Task createNotification(string userId, string content);
    }
}