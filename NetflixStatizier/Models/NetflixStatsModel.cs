using System;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Models
{
    public class NetflixStatsModel
    {
        public decimal TotalViewedHours { get; set; }

        public decimal SeriesViewedHours { get; set; }

        public decimal MoviesViewedHours { get; set; }

        public decimal SeriesEpisodesViewedItemsCount { get; set; }

        public decimal MoviesViewedCount { get; set; }

        public NetflixPlayback FirstWatchedMovie { get; set; }

        public NetflixPlayback FirstWatchedSeriesEpisode { get; set; }
    }
}
