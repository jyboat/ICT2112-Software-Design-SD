using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models.Entities
{
    [FirestoreData]  // Marks the class as Firestore-specific
    public class Notification
    {
        // Firestore properties
        [FirestoreProperty]
        private int NotificationId { get; set; }

        [FirestoreProperty]
        private int UserId { get; set; }

        // [FirestoreProperty]
        // private string Type { get; set; }

        [FirestoreProperty]
        private List<string> Method { get; set; }

        // [FirestoreProperty]
        // private DateTime Timing { get; set; }

        [FirestoreProperty]
        private string Content { get; set; }

        // Getter and Setter Methods
        private int GetNotificationId() => NotificationId;
        private void SetNotificationId(int id) => NotificationId = id;

        private int GetUserId() => UserId;
        private void SetUserId(int id) => UserId = id;

        // private string GetType() => Type;
        // private void SetType(string type) => Type = type;

        private List<string> GetMethod() => Method;
        private void SetMethod(List<string> method) => Method = method;

        // private DateTime GetTiming() => Timing;
        // private void SetTiming(DateTime timing) => Timing = timing;

        private string GetContent() => Content;
        private void SetContent(string content) => Content = content;

        // Public function to retrieve notification details as a dictionary
        public Dictionary<string, object> GetNotificationDetails()
        {
            return new Dictionary<string, object>
            {
                { "notificationId", GetNotificationId() },
                { "userId", GetUserId() },
                { "method", GetMethod() },
                { "content", GetContent() }
            };
        }

        // Static method to create a new Notification object with details
        public static Notification SetNotificationDetails(int notificationId, int userId, List<string> method, string content)
        {
            // Create a new Notification object
            Notification notification = new Notification();
            
            // Set the details using private setters
            notification.SetNotificationId(notificationId);
            notification.SetUserId(userId);
            notification.SetMethod(method);
            notification.SetContent(content);

            return notification;  // Return the created notification object
        }

        // // Public function to retrieve notification details as a dictionary
        // public Dictionary<string, object> GetNotificationDetails()
        // {
        //     return new Dictionary<string, object>
        //     {
        //         { "notificationId", GetNotificationId() },
        //         { "userId", GetUserId() },
        //         { "type", GetType() },
        //         { "method", GetMethod() },
        //         { "timing", GetTiming() },
        //         { "content", GetContent() }
        //     };
        // }


        // // Static method to create a new Notification_SDM object with details
        // public static Notification SetNotificationDetails(int notificationId, int userId, string type, List<string> method, DateTime timing, string content)
        // {
        //     // Create a new Notification_SDM object
        //     Notification_SDM notification = new Notification_SDM();
            
        //     // Set the details using private setters
        //     notification.SetNotificationId(notificationId);
        //     notification.SetUserId(userId);
        //     notification.SetType(type);
        //     notification.SetMethod(method);
        //     notification.SetTiming(timing);
        //     notification.SetContent(content);

        //     return notification;  // Return the created notification object
        // }
    }
}
