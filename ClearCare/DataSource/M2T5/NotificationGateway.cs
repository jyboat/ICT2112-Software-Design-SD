using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.DataSource
{
    public class NotificationGateway : AbstractDatabaseSubject
    {
        private readonly FirestoreDb _db;
        public NotificationGateway()
        {
            _db = FirebaseService.Initialize();
        }

        // fetchNotification() retrieves all documents in the "Notification" collection, extracts the "notificationId" from each, and returns the highest integer value. If no documents exist, it returns 0.
        public async Task<int> fetchNotification()
        {
            int highestId = 0;
            CollectionReference notificationRef = _db.Collection("Notification");
            QuerySnapshot snapshot = await notificationRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (data.ContainsKey("notificationId"))
                    {
                        int id = Convert.ToInt32(data["notificationId"]);
                        if (id > highestId)
                        {
                            highestId = id;
                        }
                    }
                }
            }

            // Notify observers that the highest ID has been fetched.
            notifyObservers($"Highest Notification ID fetched: {highestId}");
            return highestId;
        }

        // createNotification() creates a new document in the "Notification" collection using the notification's NotificationId as the document ID.
        public async Task<bool> createNotification(Notification notification)
        {
            try
            {
                DocumentReference docRef = _db.Collection("Notification").Document();
                await docRef.SetAsync(notification.GetNotificationDetails());
                Console.WriteLine("[NotificationGateway] Notification created in Firestore.");
                // Notify observers about the successful creation
                notifyObservers(true);
                return true;
            }
            catch (Exception ex)
            {
                // Log any error that occurred during the creation
                Console.WriteLine($"Error while creating notification: {ex.Message}");
                // Notify observers about the failure
                notifyObservers(false);
                return false;
            }
        }
    }
}
