using System;

namespace FlixStats.Helper
{
    public struct Time
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Time(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public override string ToString() => $"{Hours}:{Minutes}";

        public static Time FromHours(double hours)
        {
            var timespan = TimeSpan.FromHours(hours);
            return new Time((int)timespan.TotalHours, timespan.Minutes);
        }

        public static Time FromHours(decimal hours)
        {
            var timespan = TimeSpan.FromHours((double)hours);
            return new Time((int)timespan.TotalHours, timespan.Minutes);
        }

        public static Time FromMinutes(double minutes)
        {
            var timespan = TimeSpan.FromMinutes(minutes);
            return new Time((int)timespan.TotalHours, timespan.Minutes);
        }

        public static Time FromMinutes(decimal minutes)
        {
            var timespan = TimeSpan.FromMinutes((double)minutes);
            return new Time((int)timespan.TotalHours, timespan.Minutes);
        }
    }
}
