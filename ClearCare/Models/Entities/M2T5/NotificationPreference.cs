using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
    public class NotificationPreference
    {
        public string UserID { get; set; }
        public string Preference { get; set; }
        public string Methods { get; set; }
        
        // DND Settings - stored as comma-separated string
        public string DndDays { get; set; }    // Comma-separated days (e.g., "Monday,Tuesday,Wednesday")
        public TimeRange DndTimeRange { get; set; }      // Time range for DND (start time, end time)

        // Constructor to accept both UserID, Preference, Methods, and DND settings
        public NotificationPreference(string userId, string preference, string methods, string dndDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", TimeRange dndTimeRange = null)
        {
            UserID = userId;
            Preference = preference;
            Methods = methods;
            DndDays = string.IsNullOrEmpty(dndDays) ? "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday" : dndDays; // Default DND days to Monday to Sunday
            DndTimeRange = dndTimeRange ?? new TimeRange(TimeSpan.Zero, TimeSpan.FromHours(24)); // Default time range 24 hours
        }
    }

    // TimeRange class to represent the DND time range (start time and end time)
    public class TimeRange
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public TimeRange()
        {
            Start = TimeSpan.Zero;
            End = TimeSpan.FromHours(24); // Default to 24 hours (00:00 to 23:59)
        }

        // Constructor to set specific time range
        public TimeRange(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        // Helper method to check if a given time is within the DND range
        public bool IsTimeInRange(TimeSpan currentTime)
        {
            return currentTime >= Start && currentTime <= End;
        }
    }
}

