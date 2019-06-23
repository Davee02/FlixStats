using System;

namespace NetflixStatizier.Helper
{
    public static class Extensions
    {
        public static TimeSpan RoundToNearest(this TimeSpan a, TimeSpan roundTo)
        {
            long ticks = (long)(Math.Round(a.Ticks / (double)roundTo.Ticks) * roundTo.Ticks);
            return new TimeSpan(ticks);
        }

        public static bool IsFullHour(this TimeSpan a) => a.Minutes == 0 && a.Seconds == 0 && a.Milliseconds == 0;
    }
}
