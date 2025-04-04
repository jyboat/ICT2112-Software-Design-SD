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

        // createNotification() creates a new document in the "Notification" collection using the notification's NotificationId as the document ID.
        public async Task<bool> createNotification(Notification notification)
        {
            try
            {   
                DocumentReference docRef = _db.Collection("Notification").Document();
                await docRef.SetAsync(notification.getNotificationDetails());
                Console.WriteLine("[NotificationGateway] Notification created in Firestore.");
                return true;
            }
            catch (Exception ex)
            {
                // Log any error that occurred during the creation
                Console.WriteLine($"Error while creating notification: {ex.Message}");
                return false;
            }
        }

        public async Task fetchNotifications()
        {
            try
            {
                Console.WriteLine("Fetching all notifications from Firestore.");

                // Query Firestore for all notifications (no time range filter)
                var query = _db.Collection("Notification");

                var snapshot = await query.GetSnapshotAsync();
                List<Notification> notifications = new List<Notification>();

                foreach (var document in snapshot.Documents)
                {
                    // Check the document data
                    var data = document.ToDictionary();

                    // Map Firestore data to Notification entity
                    var notification = fromFirestoreData(data);

                    notifications.Add(notification);

                    try
                    {
                        await document.Reference.DeleteAsync();
                        Console.WriteLine($"Notification with ID {document.Id} deleted from Firestore.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting notification with ID {document.Id}: {ex.Message}");
                    }
                }

                // Handle notifications (e.g., notify observers or return the list)
                notifyObservers(notifications);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
            }
        }

        private Notification fromFirestoreData(Dictionary<string, object> data)
        {
            // Retrieve data from Firestore
            string userId = data.ContainsKey("userId") ? data["userId"].ToString() : "";
            string method = data.ContainsKey("method") ? data["method"].ToString() : "";
            string content = data.ContainsKey("content") ? data["content"].ToString() : "";
            string email = data.ContainsKey("email") ? data["email"].ToString() : "";
            string phone = data.ContainsKey("phone") ? data["phone"].ToString() : "";

            // Convert 'timing' directly assuming it's always a Firestore Timestamp
            DateTime timing = default(DateTime); // Default if 'timing' is not found or if not a Timestamp

            if (data.ContainsKey("timing") && data["timing"] is Google.Cloud.Firestore.Timestamp timestamp)
            {
                timing = timestamp.ToDateTime();  // Converts Firestore Timestamp to DateTime
            }

            // Use the SetNotificationDetails method to create a Notification object
            Notification notification = Notification.setNotificationDetails(userId, method, timing, content, email, phone);
            return notification;
        }
    }
}