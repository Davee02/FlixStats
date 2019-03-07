using System;

namespace NetflixStatizier.Helper
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
            return new Time((int)timespan.TotalHours, (int)timespan.Minutes);
        }

        public static Time FromMinutes(double minutes)
        {
            var timespan = TimeSpan.FromMinutes(minutes);
            return new Time((int)timespan.TotalHours, (int)timespan.Minutes);
        }
    }
}
