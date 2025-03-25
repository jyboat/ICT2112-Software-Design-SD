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
                    Console.WriteLine($"Document Data: {string.Join(", ", data.Select(kv => $"{kv.Key}: {kv.Value}"))}");

                    // Map Firestore data to Notification entity
                    var notification = FromFirestoreData(data);

                    Console.WriteLine($"GATEWAY notification timing: {notification.GetTiming()}");

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

        private Notification FromFirestoreData(Dictionary<string, object> data)
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
            Notification notification = Notification.SetNotificationDetails(userId, method, timing, content, email, phone);


            return notification;
        }

    }
}

        // // fetchNotification() retrieves all documents in the "Notification" collection, extracts the "notificationId" from each, and returns the highest integer value. If no documents exist, it returns 0.
        // public async Task<int> fetchNotification()
        // {
        //     int highestId = 0;
        //     CollectionReference notificationRef = _db.Collection("Notification");
        //     QuerySnapshot snapshot = await notificationRef.GetSnapshotAsync();

        //     foreach (DocumentSnapshot document in snapshot.Documents)
        //     {
        //         if (document.Exists)
        //         {
        //             Dictionary<string, object> data = document.ToDictionary();
        //             if (data.ContainsKey("notificationId"))
        //             {
        //                 int id = Convert.ToInt32(data["notificationId"]);
        //                 if (id > highestId)
        //                 {
        //                     highestId = id;
        //                 }
        //             }
        //         }
        //     }

        //     // Notify observers that the highest ID has been fetched.
        //     notifyObservers($"Highest Notification ID fetched: {highestId}");
        //     return highestId;
        // }

                // // This method fetches notifications from Firestore with sendTime within the next hourly cache interval
        // public async Task fetchNotifications(DateTime intervalStart, DateTime intervalEnd)
        // {
        //     try
        //     {
        //         Console.WriteLine($"Gateway: {intervalStart.ToUniversalTime()}, {intervalEnd.ToUniversalTime()}");
        //         // Query Firestore for notifications with a sendTime within the specified range
        //         var query = _db.Collection("Notification")
        //                        .WhereGreaterThanOrEqualTo("sendTime", intervalStart.ToUniversalTime())
        //                        .WhereLessThanOrEqualTo("sendTime", intervalEnd.ToUniversalTime());

        //         var snapshot = await query.GetSnapshotAsync();
        //         List<Notification> notifications = new List<Notification>();

        //         foreach (var document in snapshot.Documents)
        //         {
        //             var notification = document.ConvertTo<Notification>();
        //             notifications.Add(notification);

        //             // try
        //             // {
        //             //     await document.Reference.DeleteAsync();
        //             //     Console.WriteLine($"Notification with ID {document.Id} deleted from Firestore.");
        //             // }
        //             // catch (Exception ex)
        //             // {
        //             //     Console.WriteLine($"Error deleting notification with ID {document.Id}: {ex.Message}");
        //             // }
        //         }

        //         // Notify observers about the fetched notifications
        //         notifyObservers(notifications);

        //         Console.WriteLine("[NotificationGateway] Fetched notifications from Firestore.");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error while fetching notifications: {ex.Message}");
        //     }
        // }