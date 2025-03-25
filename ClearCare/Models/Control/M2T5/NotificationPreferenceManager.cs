using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    public class NotificationPreferenceManager: IDatabaseObserver, INotificationPreferences
    {
        private readonly NotificationPreferenceGateway _dataGateway;

        public NotificationPreferenceManager()
        {
            _dataGateway = new NotificationPreferenceGateway();
            _dataGateway.attachObserver(this);
        }

        public async Task SaveNotificationPreferenceAsync(string userId, string preference, string methods, string dndDays, TimeRange dndTimeRange)
        {
            Console.WriteLine($"Saving preference for UserID {userId}: {preference}");

            var preferenceEntity = new NotificationPreference(userId, preference, methods, dndDays, dndTimeRange);
            await _dataGateway.SaveNotificationPreferenceAsync(preferenceEntity);

            Console.WriteLine($"Preference for UserID {userId} saved successfully.");
        }

        public async Task<List<NotificationPreference>> GetNotificationPreferenceAsync(string userId)
        {
            Console.WriteLine($"Fetching notification preferences for UserID {userId}");

            // Call the interface method to get preferences for the user
            var preferences = await _dataGateway.GetNotificationPreferencesAsync();

            // Filter by userId to return the relevant preference
            var userPreference = preferences.Where(p => p.GetUserID() == userId).ToList();


            Console.WriteLine($"Fetched {userPreference.Count} preferences for UserID {userId}");
            return userPreference;
        }

        public void update(Subject subject, object data)
        {
            if (data is bool isSuccess)
            {
                // Handle success/failure of createNotification
                if (isSuccess)
                {
                    Console.WriteLine("[PreferenceManager] Notification updated successfully.");
                }
                else
                {
                    Console.WriteLine("[PreferenceManager] Notification update failed.");
                }
            }
        }
    }
}
