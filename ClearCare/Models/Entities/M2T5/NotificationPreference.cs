using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
    public class NotificationPreference
    {
        private string UserID { get; set; }
        private string Methods { get; set; }
        private string DndDays { get; set; }
        private TimeRange DndTimeRange { get; set; }

        public NotificationPreference(string userId, string methods, string dndDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", TimeRange dndTimeRange = null)
        {
            UserID = userId;
            Methods = methods;
            DndDays = string.IsNullOrEmpty(dndDays) ? "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday" : dndDays;
            DndTimeRange = dndTimeRange ?? new TimeRange(TimeSpan.Zero, TimeSpan.FromHours(24));
        }

        public string getUserID() => UserID;
        public string getMethods() => Methods;
        public string getDndDays() => DndDays;
        public TimeRange getDndTimeRange() => DndTimeRange;
    }
}
