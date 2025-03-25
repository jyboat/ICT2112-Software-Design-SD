using System;
using System.Threading.Tasks;
using System.Timers;

namespace ClearCare.Models.Control
{
    public class NotificationScheduler
    {
        private readonly NotificationManager _notificationManager;
        private readonly System.Timers.Timer _timer;

        public NotificationScheduler(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            // Set timer to fire every 1 minute (60,000 ms)
            _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += async (sender, e) => await TimerElapsed();
            _timer.AutoReset = true;
            _timer.Start();
        }

        private async Task TimerElapsed()
        {
            DateTime now = DateTime.UtcNow;

            // Check the cache and send notifications that are due.
            await _notificationManager.checkCacheAndSend();

            // If current time is at the hourly boundary (e.g., minute == 0), flush the cache.
            if (now.Minute == 0)
            {
                Console.WriteLine("[NotificationScheduler] Hourly interval reached. Flushing cache.");
                await _notificationManager.flushCache();

                // Fetch notifications for the next hourly interval and add them to the cache.
                await _notificationManager.getNotifications();
                // Optionally, reload cache from the database for notifications due before the next hourly expiry.
                // (This part can be added if required.)
            }
        }
    }
}
