using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control {
    public class NotificationManager : INotification, IDatabaseObserver {
        private readonly NotificationGateway _notificationGateway;
        private readonly INotificationSender _notificationSenderEmail; // Dependency for sending emails
        private readonly INotificationSender _notificationSenderEmailSMS; // Dependency for sending SMS
        private readonly INotificationPreferences _notificationPreference; // Dependency for accessing notification preferences

        // Constructor: initialize NotificationGateway, attach as observer, and inject INotificationSender
        public NotificationManager() {
            _notificationGateway = new NotificationGateway();
            _notificationGateway.attachObserver(this);

            _notificationSenderEmail = new NotificationEmailService(); // Direct initialization
            _notificationSenderEmailSMS = new SMSNotificationDecorator(_notificationSenderEmail);

            _notificationPreference = new NotificationPreferenceManager();
        }

        // createNotification: generates a new notification, stores it in the database, then triggers sending.
        public async Task createNotification(string userId, string content) {   
            // string methods = "Email"; // Placeholder for INotificationPreferences
            // string methods = "Email,SMS"; // Placeholder for INotificationPreferences
            // Check doNotDisturb, if current time not inside, sendNow = True
            //bool sendNow = true; // Placeholder
            //bool sendNow = false; // Placeholder

            var (methods, sendNow, sendTime) = await checkPreference(userId);
            DateTime timing = DateTime.UtcNow;

            string email = "booyancong@gmail.com"; // Placeholder
            string phone = "+6500000000"; // Placeholder
            Console.WriteLine(TimeZoneInfo.Local.Id);
            sendTime = sendTime.ToUniversalTime();
            Console.WriteLine($"UserID: {userId}, Email: {email}, Phone: {phone}, Methods: {methods}, SendNow: {sendNow}, Send Time: {sendTime}");

            if (sendNow) {
                await sendNotification(methods, email, phone, content);
            } else  {   
                // Else, calculate sendTime. Then make object and check whether to put in cache or database.
                // DateTime sendTime = DateTime.UtcNow.AddMinutes(1); // Placeholder
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

        // checkPreferences retrieves user preference and checks if notification should be sent now
        private async Task<(string methods, bool sendNow, DateTime sendTime)> checkPreference(string userId) {
            var preference = await _notificationPreference.GetNotificationPreferenceAsync(userId);

            var userPreference = preference.First();
            // Get the notification methods, DND days, and DND time range from the preference
            var methods = userPreference.GetMethods();
            var dndDaysString = userPreference.GetDndDays();
            var dndTimeRange = userPreference.GetDndTimeRange();

            // Parse DndDays into a list (e.g., "Monday,Tuesday,Wednesday" becomes a list of strings)
            var dndDays = dndDaysString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(day => day.Trim()).ToList();

            // Determine the current day and time
            var currentDay = DateTime.Now.DayOfWeek.ToString();  // e.g., "Monday"
            var currentTime = DateTime.Now.TimeOfDay;

            // Check if the current day is one of the DND days
            bool isTodayDnd = dndDays.Contains(currentDay);

            // Check if the current time is within the DND time range
            bool isTimeInRange = dndTimeRange.IsTimeInRange(currentTime);

            // If today is a DND day and the current time is within the specified range, do not send now
            bool sendNow = !(isTodayDnd && isTimeInRange);

            var sendTime = DateTime.UtcNow;
            Console.WriteLine($"1NotificationCheckPreference: Send Time: {sendTime}");

            // If sending is blocked, compute the next valid time
            if (!sendNow)
            {   
                Console.WriteLine($"2NotificationCheckPreference: dndTimeRange: {dndTimeRange.GetEndTime()}, Send Time: {DateTime.UtcNow.Date.Add(dndTimeRange.GetEndTime())}");
                DateTime localDate = DateTime.Now.Date;
                DateTime localSendTime = localDate.Add(dndTimeRange.GetEndTime());

                sendTime = localSendTime.ToUniversalTime();
            }
            Console.WriteLine($"NotificationCheckPreference: Send Time: {sendTime}");
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
            var pendingNotifications = NotificationCache.GetAllNotifications();
            foreach (var notification in pendingNotifications)
            {
                await _notificationGateway.createNotification(notification);
            }
            NotificationCache.ClearCache();
        }

        // checkCacheAndSend: checks the cache for due notifications, sends them, and removes them from the cache.
        public async Task checkCacheAndSend() {
            DateTime now = DateTime.UtcNow;
            var dueNotifications = NotificationCache.GetDueNotifications(now);
            foreach (var notification in dueNotifications)
            {
                Dictionary<string, object> details = notification.GetNotificationDetails();
                await sendNotification(details["method"].ToString(), details["email"].ToString(), details["phone"].ToString(), details["content"].ToString());
            }
            NotificationCache.RemoveNotifications(dueNotifications);
        }

        // Scheduler should call getNotifications to fetch notifications for the next interval
        public async Task getNotifications()
        {
            // DateTime intervalStart = DateTime.UtcNow;
            // DateTime intervalEnd = NotificationCache.CurrentIntervalEnd; // Fetch notifications for the next hourly interval
            // Console.WriteLine($"NotificationManager: intervalEnd: {intervalEnd}");
            // DateTime intervalEndTest = DateTime.UtcNow.AddHours(1);
            // Console.WriteLine($"NotificationManager: intervalEndTest: {intervalEndTest}");
            await _notificationGateway.fetchNotifications();
        }

        // update: Observer method called by NotificationGateway when a change occurs.
        public void update(Subject subject, object data) {
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
            } else if (data is List<Notification> notifications)  {
                Console.WriteLine("[NotificationManager] Notifications fetched from Firestore.");
                DateTime intervalStart = DateTime.UtcNow;
                DateTime intervalEnd = NotificationCache.CurrentIntervalEnd;

                // Add fetched notifications to the cache or store them in the database
                foreach (var notification in notifications)
                {
                    Console.WriteLine($"Checking notification with timing: {notification.GetTiming()}");

                    // Check if the notification's timing is within the current interval
                    if (notification.GetTiming() >= intervalStart && notification.GetTiming() <= intervalEnd)
                    {
                        // If within the interval, add to cache
                        NotificationCache.AddNotification(notification);
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
