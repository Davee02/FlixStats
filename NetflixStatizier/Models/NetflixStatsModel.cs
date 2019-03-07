using System.Collections.Generic;
using NetflixStatizier.Helper;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Models
{
    public class NetflixStatsModel
    {
        public Time TotalViewedTime { get; set; }

        public Time SeriesViewedTime { get; set; }

        public Time MoviesViewedTime { get; set; }

        public decimal SeriesEpisodesViewedItemsCount { get; set; }

        public decimal MoviesViewedCount { get; set; }

        public NetflixPlayback FirstWatchedMovie { get; set; }

        public NetflixPlayback FirstWatchedSeriesEpisode { get; set; }

        public Dictionary<NetflixSerie, Time> ViewedTimePerSerie { get; set; }
    }
}
