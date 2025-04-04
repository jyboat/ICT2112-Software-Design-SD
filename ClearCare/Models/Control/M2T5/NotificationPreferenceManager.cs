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
        private List<NotificationPreference> _cachedNotificationPreference = new List<NotificationPreference>();

        public NotificationPreferenceManager()
        {
            _dataGateway = new NotificationPreferenceGateway();
            _dataGateway.attachObserver(this);
        }

        public async Task updateNotificationPreferences(string userId, string methods, string dndDays, TimeRange dndTimeRange)
        {
            Console.WriteLine($"NotificationPreferenceManager: Saving preference for UserID {userId}");

            var preferenceEntity = new NotificationPreference(userId, methods, dndDays, dndTimeRange);
            await _dataGateway.updateNotificationPreferences(preferenceEntity);

            Console.WriteLine($"NotificationPreferenceManager: Preference for UserID {userId} saved successfully.");
        }

        public async Task<List<NotificationPreference>> getNotificationPreferences(string userId)
        {
            Console.WriteLine($"NotificationPreferenceManager: Fetching notification preferences for UserID {userId}");
            if (_cachedNotificationPreference == null || !_cachedNotificationPreference.Any())
            {
                await fetchNotificationPreferences(); // trigger async update and populate cache
            }
             var preferences =  _cachedNotificationPreference;

            // Filter by userId to return the relevant preference
            var userPreference = preferences.Where(p => p.GetUserID() == userId).ToList();
            Console.WriteLine($"NotificationPreferenceManager: Fetched {userPreference.Count} preferences for UserID {userId}");

            if (userPreference.Count == 0) {
                var defaultTimeRange = new TimeRange(TimeSpan.FromHours(0), TimeSpan.FromHours(1));
                var defaultUserPreference = new NotificationPreference(userId, "email", "none", defaultTimeRange);
                Console.WriteLine($"NotificationPreferenceManager: Returning default preferences");
                return new List<NotificationPreference> { defaultUserPreference };
            }
            Console.WriteLine($"NotificationPreferenceManager: Returning user preferences");
            return userPreference;
        }

        public async Task fetchNotificationPreferences()
        {
            await _dataGateway.fetchNotificationPreferences();
        }

        public void update(Subject subject, object data)
        {
            Console.WriteLine("NotificationPreferenceManager: Observer triggered! Data received from Gateway.");

            if (data is List<NotificationPreference> notificationPreference)
            {
                _cachedNotificationPreference = notificationPreference;
            }
        }
    }
}
