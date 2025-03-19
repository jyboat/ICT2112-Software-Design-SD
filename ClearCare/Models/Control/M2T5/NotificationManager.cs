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
        private readonly INotificationSender _notificationSender; // Dependency for sending emails

        // Constructor: initialize NotificationGateway, attach as observer, and inject INotificationSender
        public NotificationManager()
        {
            _notificationGateway = new NotificationGateway();
            _notificationGateway.attachObserver(this);

            // Initialize the INotificationSender directly
            _notificationSender = new NotificationEmailService(); // Direct initialization
        }
        
        // createNotification: generates a new notification, stores it in the database, then triggers sending.
        public async Task createNotification(int userId, string content)
        {
            // Generate a new notification ID.
            int notificationId = await generateNotificationId();

            List<string> methods = new List<string> { "Email" }; // Placeholder for INotificationPreferences
            Notification notification = Notification.SetNotificationDetails(
                notificationId, userId, methods, content
            );

            // Save the notification in the database and check success
            bool isCreated = await _notificationGateway.createNotification(notification);
            if (isCreated)
            {
                // Trigger sending of the notification if creation was successful
                await sendNotification("booyancong@gmail.com", content);
            }
            else
            {
                // Log failure if notification creation was unsuccessful
                Console.WriteLine("[NotificationManager] Failed to create notification.");
            }
        }

        // generateNotificationId: retrieves the highest notification ID from the database and returns it plus one.
        private async Task<int> generateNotificationId()
        {
            int highestNotificationId = await _notificationGateway.fetchNotification();
            int newId = (highestNotificationId != null) ? highestNotificationId + 1 : 1;
            return newId;
        }

        // sendNotification triggers the actual email sending via INotificationSender
        private async Task sendNotification(string email, string content)
        {
            // Call the INotificationSender to send the email
            await _notificationSender.sendNotification(email, content);
            Console.WriteLine("[NotificationManager] Email notification sent.");
        }

        // update: Observer method called by NotificationGateway when a change occurs.
        public void update(Subject subject, object data)
        {
            if (data is string notificationStatus)
            {
                // Handle fetchNotification update
                if (notificationStatus.Contains("Highest Notification ID fetched"))
                {
                    Console.WriteLine($"[NotificationManager] {notificationStatus}");
                }
                else if (notificationStatus == "Notification Created")
                {
                    Console.WriteLine("[NotificationManager] Notification was successfully created.");
                }
                else
                {
                    Console.WriteLine("[NotificationManager] Unrecognized status update.");
                }
            }
            else if (data is bool isSuccess)
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