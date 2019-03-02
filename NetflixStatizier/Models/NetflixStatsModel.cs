using System;

namespace NetflixStatizier.Models
{
    public class NetflixStatsModel
    {
        public decimal TotalViewedHours { get; set; }

        public decimal SeriesViewedHours { get; set; }

        public decimal MoviesViewedHours { get; set; }

        public decimal SeriesEpisodesViewedItemsCount { get; set; }

        public decimal MoviesViewedCount { get; set; }

        public DateTime FirstWatchedItemDateTime { get; set; }

        public DateTime LastWatchedItemDateTime { get; set; }
    }
}
