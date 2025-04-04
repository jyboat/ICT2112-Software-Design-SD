using System;

namespace ClearCare.Models.Entities
{
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

        public TimeSpan getStartTime() => Start;
        public TimeSpan getEndTime() => End;

        public bool isTimeInRange(TimeSpan currentTime)
        {
            return currentTime >= Start && currentTime <= End;
        }
    }
}
