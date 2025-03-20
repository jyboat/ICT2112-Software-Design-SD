using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

using System.Threading.Tasks;
using ClearCare.Interfaces;

using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class NotificationPreferenceManager
    {
        private readonly NotificationPreferenceGateway _dataGateway;

        public NotificationPreferenceManager()
        {
            _dataGateway = new NotificationPreferenceGateway(); // ✅ Correct way to instantiate
        }

        public async Task SaveNotificationPreferenceAsync(string userId, string preference, List<string> methods)
        {
            Console.WriteLine($"Saving preference for UserID {userId}: {preference}");

            // Save the preference (example for Firestore)
            var preferenceEntity = new NotificationPreference(userId, preference, methods);
            await _dataGateway.SaveNotificationPreferenceAsync(preferenceEntity);

            // Log again after saving
            Console.WriteLine($"Preference for UserID {userId} saved successfully.");
        }

    }
}

