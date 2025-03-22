using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class NotificationManager : INotification, IDatabaseObserver
    {
        private readonly NotificationGateway _notificationGateway;
        private readonly INotificationSender _notificationSenderEmail; // Dependency for sending emails
        private readonly INotificationSender _notificationSenderEmailSMS; // Dependency for sending SMS

        // Constructor: initialize NotificationGateway, attach as observer, and inject INotificationSender
        public NotificationManager()
        {
            _notificationGateway = new NotificationGateway();
            _notificationGateway.attachObserver(this);

            // Initialize the INotificationSender directly
            _notificationSenderEmail = new NotificationEmailService(); // Direct initialization
            _notificationSenderEmailSMS = new SMSNotificationDecorator(_notificationSenderEmail);
        }

        // createNotification: generates a new notification, stores it in the database, then triggers sending.
        public async Task createNotification(int userId, string content)
        {   
            string methods = "Email"; // Placeholder for INotificationPreferences
            // string methods = "Email,SMS"; // Placeholder for INotificationPreferences
            string email = "example@gmail.com"; // Placeholder
            string phone = "+6500000000"; // Placeholder
            DateTime timing = DateTime.UtcNow;
            // Check doNotDisturb, if current time not inside, sendNow = True
            bool sendNow = true; // Placeholder
            //bool sendNow = false; // Placeholder

            if (sendNow) {
                await sendNotification(methods, email, phone, content);
            } else  {   // Else, calculate sendTime. Then make object and check whether to put in cache or database.
                DateTime sendTime = DateTime.UtcNow.AddMinutes(1); // Placeholder
                Notification notification = Notification.SetNotificationDetails(userId, methods, sendTime, content, email, phone);

                // Check if scheduled time is within the current cache interval.
                if (NotificationCache.IsWithinCurrentInterval(sendTime)) {
                    NotificationCache.AddNotification(notification);
                } else {
                    // If not within current cache interval, write it directly to DB.
                    await _notificationGateway.createNotification(notification);
                }
            }
        }

        // sendNotification triggers the relevant senders
        private async Task sendNotification(string methods, string email, string phone, string content)
        {   
            if (methods == "Email") {
                // Call the INotificationSender to send the email
                await _notificationSenderEmail.sendNotification(email, phone, content);
                Console.WriteLine("[NotificationManager] Email notification sent.");
            } else if (methods == "Email,SMS") {
                // Call the INotificationSender to send the email
                await _notificationSenderEmailSMS.sendNotification(email, phone, content);
                Console.WriteLine("[NotificationManager] Email and SMS notification sent.");
            } else {
                Console.WriteLine("[NotificationManager] Invalid methods");
            }
        }

        // // sendNotification triggers the actual email sending via INotificationSender
        // private async Task sendNotificationEmail(string email, string phone, string content)
        // {
        //     // Call the INotificationSender to send the email
        //     await _notificationSenderEmail.sendNotification(email, phone, content);
        //     Console.WriteLine("[NotificationManager] Email notification sent.");
        // }

        // // sendNotification triggers the actual SMS sending via INotificationSender
        // private async Task sendNotificationEmailSMS(string email, string phone, string content)
        // {
        //     // Call the INotificationSender to send the email
        //     await _notificationSenderEmailSMS.sendNotification(email, phone, content);
        //     Console.WriteLine("[NotificationManager] Email and SMS notification sent.");
        // }

        // flushCache: writes all notifications from the cache to the database and clears the cache.
        public async Task flushCache()
        {
            var pendingNotifications = NotificationCache.GetAllNotifications();
            foreach (var notification in pendingNotifications)
            {
                await _notificationGateway.createNotification(notification);
            }
            NotificationCache.ClearCache();
        }

        // checkCacheAndSend: checks the cache for due notifications, sends them, and removes them from the cache.
        public async Task checkCacheAndSend()
        {
            DateTime now = DateTime.UtcNow;
            var dueNotifications = NotificationCache.GetDueNotifications(now);
            foreach (var notification in dueNotifications)
            {
                Dictionary<string, object> details = notification.GetNotificationDetails();
                await sendNotification(details["method"].ToString(), details["email"].ToString(), details["phone"].ToString(), details["content"].ToString());
            }
            NotificationCache.RemoveNotifications(dueNotifications);
        }

        // update: Observer method called by NotificationGateway when a change occurs.
        public void update(Subject subject, object data)
        {
            if (data is bool isSuccess)
            {
                // Handle success/failure of createNotification
                if (isSuccess)
                {
                    Console.WriteLine("[NotificationManager] Notification created successfully.");
                }
                else
                {
                    Console.WriteLine("[NotificationManager] Notification creation failed.");
                }
            }
        }
    }
}

        // // generateNotificationId: retrieves the highest notification ID from the database and returns it plus one.
        // private async Task<int> generateNotificationId()
        // {
        //     int highestNotificationId = await _notificationGateway.fetchNotification();
        //     int newId = (highestNotificationId != null) ? highestNotificationId + 1 : 1;
        //     return newId;
        // }

        // // createNotification: generates a new notification, stores it in the database, then triggers sending.
        // public async Task createNotification(int userId, string content)
        // {
        //     // Generate a new notification ID.
        //     int notificationId = await generateNotificationId();

        //     List<string> methods = new List<string> { "Email" }; // Placeholder for INotificationPreferences
        //     Notification notification = Notification.SetNotificationDetails(
        //         notificationId, userId, methods, content
        //     );

        //     // Save the notification in the database and check success
        //     bool isCreated = await _notificationGateway.createNotification(notification);
        //     if (isCreated)
        //     {
        //         // Trigger sending of the notification if creation was successful
        //         await sendNotification("booyancong@gmail.com", content);
        //     }
        //     else
        //     {
        //         // Log failure if notification creation was unsuccessful
        //         Console.WriteLine("[NotificationManager] Failed to create notification.");
        //     }
        // }