using Google.Cloud.Firestore;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using ClearCare.Models.Interface;
using System.Threading.Tasks;
using System.Data.Common;

namespace ClearCare.DataSource
{
    public class NotificationPreferenceGateway
    {
        private readonly FirestoreDb _db;

        public NotificationPreferenceGateway()
        {
            _db = FirebaseService.Initialize();
        }

public async Task SaveNotificationPreferenceAsync(NotificationPreference preference)
{
    var preferenceCollection = _db.Collection("notification_preferences");

    DocumentReference docRef = preferenceCollection.Document(preference.UserID.ToString());

    var data = new Dictionary<string, object>
    {
        { "UserID", preference.UserID },
        { "Preference", preference.Preference}, // Save the actual preference
        {  "Methods", preference.Methods}
    };

    await docRef.SetAsync(data);
}

    }
}
