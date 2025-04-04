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

        public async Task UpdateNotificationPreferences(string userId, string methods, string dndDays, TimeRange dndTimeRange)
        {
            Console.WriteLine($"Saving preference for UserID {userId}");

            var preferenceEntity = new NotificationPreference(userId, methods, dndDays, dndTimeRange);
            await _dataGateway.UpdateNotificationPreferences(preferenceEntity);

            Console.WriteLine($"Preference for UserID {userId} saved successfully.");
        }

        public async Task<List<NotificationPreference>> GetNotificationPreferences(string userId)
        {
            Console.WriteLine($"Fetching notification preferences for UserID {userId}");

            // Call the interface method to get preferences for the user
            var preferences = await _dataGateway.GetNotificationPreferences();

            // Filter by userId to return the relevant preference
            var userPreference = preferences.Where(p => p.GetUserID() == userId).ToList();
            Console.WriteLine($"Fetched {userPreference.Count} preferences for UserID {userId}");

            if (userPreference.Count == 0) {
                var defaultTimeRange = new TimeRange(TimeSpan.FromHours(0), TimeSpan.FromHours(1));
                var defaultUserPreference = new NotificationPreference(userId, "email", "none", defaultTimeRange);
                Console.WriteLine($"Returning default preferences");
                return new List<NotificationPreference> { defaultUserPreference };
            }
            Console.WriteLine($"Returning user preferences");
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
