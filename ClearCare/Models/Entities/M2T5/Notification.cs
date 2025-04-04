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
        private string getUserID() => UserId;
        private void setUserId(string id) => UserId = id;

        private string getMethod() => Method;
        private void setMethod(string method) => Method = method;

        public DateTime getTiming() => DateTime.SpecifyKind(Timing, DateTimeKind.Utc);
        private void setTiming(DateTime timing) => Timing = timing.ToUniversalTime();

        private string getContent() => Content;
        private void setContent(string content) => Content = content;
        
        private string getEmail() => Email;
        private void setEmail(string email) => Email = email;

        private string getPhone() => Phone;
        private void setPhone(string phone) => Phone = phone;


        // Public function to retrieve notification details as a dictionary
        public Dictionary<string, object> getNotificationDetails()
        {
            return new Dictionary<string, object>
            {
                {"userId", getUserID()},
                {"method", getMethod()},
                {"timing", getTiming()},
                {"content", getContent()},
                {"email", getEmail()},
                {"phone", getPhone()}
            };
        }

        // Static method to create a new Notification object with details
        public static Notification setNotificationDetails(string userId, string method, DateTime timing, string content, string email, string phone)
        {
            // Create a new Notification object
            Notification notification = new Notification();
            
            // Set the details using private setters
            notification.setUserId(userId);
            notification.setMethod(method);
            notification.setTiming(timing);
            notification.setContent(content);
            notification.setEmail(email);
            notification.setPhone(phone);
            
            // Return the created notification object
            return notification;
        }
    }
}
