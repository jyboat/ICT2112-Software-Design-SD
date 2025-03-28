using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;

namespace ClearCare.DataSource
{

    
    public class NotificationPreferenceGateway: AbstractDatabaseSubject
    {
        private readonly FirestoreDb _db;

        public NotificationPreferenceGateway()
        {
            _db = FirebaseService.Initialize();
        }

        public async Task SaveNotificationPreferenceAsync(NotificationPreference preference)
        {

            try{
            var preferenceCollection = _db.Collection("notification_preferences");
            DocumentReference docRef = preferenceCollection.Document(preference.GetUserID());


            var data = new Dictionary<string, object>
            {
                { "UserID", preference.GetUserID() },
                { "Methods", preference.GetMethods() },
                { "DndDays", preference.GetDndDays() },
                { "DndTimeRange", $"{preference.GetDndTimeRange().GetStartTime()}-{preference.GetDndTimeRange().GetEndTime()}" }

            };
            await docRef.SetAsync(data);
                notifyObservers(true);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error while creating notification: {ex.Message}");
                // Notify observers about the failure
                notifyObservers(false);
            }
        }


        public async Task<List<NotificationPreference>> GetNotificationPreferencesAsync()
        {
            var preferences = new List<NotificationPreference>();
            var preferenceCollection = _db.Collection("notification_preferences");
            QuerySnapshot snapshot = await preferenceCollection.GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();
                var userID = data["UserID"].ToString();

                // Handle Methods as comma-separated string and split it into a list
                var methods = data.ContainsKey("Methods") ? data["Methods"].ToString().Split(',').ToList() : new List<string>();

                // Get DND Days
                var dndDays = data.ContainsKey("DndDays") ? data["DndDays"].ToString() : "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday";

                // Get DND Time Range (Start-End)
                var dndTimeRange = data.ContainsKey("DndTimeRange") ? data["DndTimeRange"].ToString() : "00:00-23:59";
                var timeRangeParts = dndTimeRange.Split('-');
                var startTime = TimeSpan.Parse(timeRangeParts[0]);
                var endTime = TimeSpan.Parse(timeRangeParts[1]);
                var dndTimeRangeObj = new TimeRange(startTime, endTime);

                preferences.Add(new NotificationPreference(userID, string.Join(",", methods), dndDays, dndTimeRangeObj));
            }
            return preferences;
        }
    }
}
 