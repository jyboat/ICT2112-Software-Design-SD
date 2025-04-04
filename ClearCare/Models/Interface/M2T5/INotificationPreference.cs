using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

    namespace ClearCare.Interfaces
    {
        public interface INotificationPreferences
        {
            Task<List<NotificationPreference>> getNotificationPreferences(string userId);
        }
    }
