using System.Threading.Tasks;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;


namespace ClearCare.Models.Interface
{
    public interface INotification
    {
        // Creates a notification for the given user with the provided content.
        Task createNotification(int userId, string content);
    }
}