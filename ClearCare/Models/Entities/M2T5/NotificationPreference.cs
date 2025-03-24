using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
    public class NotificationPreference
    {
        private string UserID { get; set; }
        private string Preference { get; set; }
        private string Methods { get; set; }
        private string DndDays { get; set; }
        private TimeRange DndTimeRange { get; set; }

        public NotificationPreference(string userId, string preference, string methods, string dndDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", TimeRange dndTimeRange = null)
        {
            UserID = userId;
            Preference = preference;
            Methods = methods;
            DndDays = string.IsNullOrEmpty(dndDays) ? "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday" : dndDays;
            DndTimeRange = dndTimeRange ?? new TimeRange(TimeSpan.Zero, TimeSpan.FromHours(24));
        }

        public string GetUserID() => UserID;
        public string GetPreference() => Preference;
        public string GetMethods() => Methods;
        public string GetDndDays() => DndDays;
        public TimeRange GetDndTimeRange() => DndTimeRange;
    }

    public class TimeRange
    {
        private TimeSpan Start { get; set; }
        private TimeSpan End { get; set; }

        public TimeRange()
        {
            Start = TimeSpan.Zero;
            End = TimeSpan.FromHours(24);
        }

        public TimeRange(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        public TimeSpan GetStartTime() => Start;
        public TimeSpan GetEndTime() => End;

        public bool IsTimeInRange(TimeSpan currentTime)
        {
            return currentTime >= Start && currentTime <= End;
        }
    }
}
