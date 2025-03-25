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
        private string UserId { get; set; }

        [FirestoreProperty]
        private string Method { get; set; }

        [FirestoreProperty]
        private DateTime Timing { get; set; }

        [FirestoreProperty]
        private string Content { get; set; }

        [FirestoreProperty]
        private string Email { get; set; }

        [FirestoreProperty]
        private string Phone { get; set; }

        // Getter and Setter Methods
        private string GetUserId() => UserId;
        private void SetUserId(string id) => UserId = id;

        private string GetMethod() => Method;
        private void SetMethod(string method) => Method = method;

        public DateTime GetTiming() => DateTime.SpecifyKind(Timing, DateTimeKind.Utc);
        private void SetTiming(DateTime timing) => Timing = timing.ToUniversalTime();

        private string GetContent() => Content;
        private void SetContent(string content) => Content = content;
        
        private string GetEmail() => Email;
        private void SetEmail(string email) => Email = email;

        private string GetPhone() => Phone;
        private void SetPhone(string phone) => Phone = phone;


        // Public function to retrieve notification details as a dictionary
        public Dictionary<string, object> GetNotificationDetails()
        {
            return new Dictionary<string, object>
            {
                {"userId", GetUserId()},
                {"method", GetMethod()},
                {"timing", GetTiming()},
                {"content", GetContent()},
                {"email", GetEmail()},
                {"phone", GetPhone()}
            };
        }

        // Static method to create a new Notification object with details
        public static Notification SetNotificationDetails(string userId, string method, DateTime timing, string content, string email, string phone)
        {
            // Create a new Notification object
            Notification notification = new Notification();
            
            // Set the details using private setters
            notification.SetUserId(userId);
            notification.SetMethod(method);
            notification.SetTiming(timing);
            notification.SetContent(content);
            notification.SetEmail(email);
            notification.SetPhone(phone);
            
            // Return the created notification object
            return notification;
        }
    }
}
