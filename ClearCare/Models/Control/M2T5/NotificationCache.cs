using System;
using System.Collections.Generic;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public static class NotificationCache
    {
        private static List<Notification> _cache = new List<Notification>();

        public static DateTime CurrentIntervalEnd { get; private set; } = GetCurrentIntervalEnd();

        private static DateTime GetCurrentIntervalEnd()
        {
            DateTime now = DateTime.UtcNow;
            // Round up to the next whole hour.
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
        }

        public static bool IsWithinCurrentInterval(DateTime scheduledTime)
        {
            return scheduledTime <= CurrentIntervalEnd;
        }

        public static void AddNotification(Notification notification)
        {
            _cache.Add(notification);
            Console.WriteLine($"[NotificationCache] Notification added to cache.");
        }

        public static List<Notification> GetDueNotifications(DateTime now)
        {
            return _cache.FindAll(n => n.GetTiming() <= now);
        }

        public static void RemoveNotifications(List<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                _cache.Remove(notification);
            }
        }

        public static List<Notification> GetAllNotifications()
        {
            return new List<Notification>(_cache);
        }

        public static void ClearCache()
        {
            _cache.Clear();
            CurrentIntervalEnd = GetCurrentIntervalEnd();
        }
    }
}