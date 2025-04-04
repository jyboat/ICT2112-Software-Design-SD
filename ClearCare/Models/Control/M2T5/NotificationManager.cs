using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control {
    public class NotificationManager : INotification, IDatabaseObserver {
        private readonly NotificationGateway _notificationGateway;
        private readonly INotificationSender _notificationSenderEmail; // Dependency for sending emails
        private readonly INotificationSender _notificationSenderEmailSMS; // Dependency for sending SMS
        private readonly INotificationPreferences _notificationPreference; // Dependency for accessing notification preferences
        private readonly IUserDetails _userDetails;

        // Constructor: initialize NotificationGateway, attach as observer, and inject INotificationSender
        public NotificationManager() {
            _notificationGateway = new NotificationGateway();
            _notificationGateway.attachObserver(this);

            _notificationSenderEmail = new NotificationEmailService(); // Direct initialization
            _notificationSenderEmailSMS = new SMSNotificationDecorator(_notificationSenderEmail);

            _notificationPreference = new NotificationPreferenceManager();
            _userDetails  = new ProfileManagement();
        }

        // createNotification: generates a new notification, stores it in the database, then triggers sending.
        public async Task createNotification(string userId, string content) {   
            var (methods, sendNow, sendTime) = await checkPreference(userId);
            DateTime timing = DateTime.UtcNow;

            var userdata = await _userDetails.getUserDetails(userId);
            var user = userdata.getProfileData();

            string email = user["Email"].ToString();
            string phone = "+" + user["MobileNumber"].ToString();
            sendTime = sendTime.ToUniversalTime();

            if (sendNow) {
                await sendNotification(methods, email, phone, content);
            } else  {   
                // Else, calculate sendTime. Then make object and check whether to put in cache or database.
                // DateTime sendTime = DateTime.UtcNow.AddMinutes(1); // Placeholder
                Notification notification = Notification.setNotificationDetails(userId, methods, sendTime, content, email, phone);

                // Check if scheduled time is within the current cache interval.
                if (NotificationCache.isWithinCurrentInterval(sendTime)) {
                    NotificationCache.addNotification(notification);
                } else {
                    // If not within current cache interval, write it directly to DB.
                    await _notificationGateway.createNotification(notification);
                }
            }
        }

        // checkPreferences retrieves user preference and checks if notification should be sent now
        private async Task<(string methods, bool sendNow, DateTime sendTime)> checkPreference(string userId) {
            var preference = await _notificationPreference.getNotificationPreferences(userId);

            var userPreference = preference.First();
            // Get the notification methods, DND days, and DND time range from the preference
            var methods = userPreference.getMethods();
            var dndDaysString = userPreference.getDndDays();
            var dndTimeRange = userPreference.getDndTimeRange();

            // Parse DndDays into a list (e.g., "Monday,Tuesday,Wednesday" becomes a list of strings)
            var dndDays = dndDaysString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(day => day.Trim()).ToList();

            // Determine the current day and time
            var currentDay = DateTime.Now.DayOfWeek.ToString();  // e.g., "Monday"
            var currentTime = DateTime.Now.TimeOfDay;

            // Check if the current day is one of the DND days
            bool isTodayDnd = dndDays.Contains(currentDay);

            // Check if the current time is within the DND time range
            bool isTimeInRange = dndTimeRange.isTimeInRange(currentTime);

            // If today is a DND day and the current time is within the specified range, do not send now
            bool sendNow = !(isTodayDnd && isTimeInRange);
            var sendTime = DateTime.UtcNow;

            // If sending is blocked, compute the next valid time
            if (!sendNow)
            {   
                DateTime localDate = DateTime.Now.Date;
                DateTime localSendTime = localDate.Add(dndTimeRange.getEndTime());

                sendTime = localSendTime.ToUniversalTime();
            }
            return (methods, sendNow, sendTime);
        }

        // sendNotification triggers the relevant senders
        private async Task sendNotification(string methods, string email, string phone, string content) {   
            if (methods == "email") {
                // Call the INotificationSender to send the email
                await _notificationSenderEmail.sendNotification(email, phone, content);
                Console.WriteLine("[NotificationManager] Email notification sent.");
            } else if (methods == "sms,email") {
                // Call the INotificationSender to send the email
                await _notificationSenderEmailSMS.sendNotification(email, phone, content);
                Console.WriteLine("[NotificationManager] Email and SMS notification sent.");
            } else {
                Console.WriteLine("[NotificationManager] Invalid methods");
            }
        }

        // flushCache: writes all notifications from the cache to the database and clears the cache.
        public async Task flushCache() {
            var pendingNotifications = NotificationCache.getAllNotifications();
            foreach (var notification in pendingNotifications)
            {
                await _notificationGateway.createNotification(notification);
            }
            NotificationCache.clearCache();
        }

        // checkCacheAndSend: checks the cache for due notifications, sends them, and removes them from the cache.
        public async Task checkCacheAndSend() {
            DateTime now = DateTime.UtcNow;
            var dueNotifications = NotificationCache.getDueNotifications(now);
            foreach (var notification in dueNotifications)
            {
                Dictionary<string, object> details = notification.getNotificationDetails();
                await sendNotification(details["method"].ToString(), details["email"].ToString(), details["phone"].ToString(), details["content"].ToString());
            }
            NotificationCache.removeNotifications(dueNotifications);
        }

        // Scheduler should call getNotifications to fetch notifications for the next interval
        public async Task getNotifications()
        {
            await _notificationGateway.fetchNotifications();
        }

        // update: Observer method called by NotificationGateway when a change occurs.
        public void update(Subject subject, object data) {
            if (data is List<Notification> notifications)  {
                Console.WriteLine("[NotificationManager] Notifications fetched from Firestore.");
                DateTime intervalStart = DateTime.UtcNow;
                DateTime intervalEnd = NotificationCache.currentIntervalEnd;

                // Add fetched notifications to the cache or store them in the database
                foreach (var notification in notifications)
                {
                    Console.WriteLine($"Checking notification with timing: {notification.getTiming()}");

                    // Check if the notification's timing is within the current interval
                    if (notification.getTiming() >= intervalStart && notification.getTiming() <= intervalEnd)
                    {
                        // If within the interval, add to cache
                        NotificationCache.addNotification(notification);
                        Console.WriteLine("[NotificationManager] ADDED TO CACHE");
                    }
                    else
                    {
                        // If not within the interval, store directly in DB
                        _notificationGateway.createNotification(notification);
                        Console.WriteLine("[NotificationManager] STORED IN DB");
                    }
                }
            }
        }
    }
}
