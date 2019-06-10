using System;
using System.Collections.Generic;
using System.Linq;
using NetflixStatizier.Stats.Abstractions;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Stats
{
    public class NetflixStatsCalculator : IStatsCalculator<NetflixPlayback, NetflixSerie, NetflixEpisode>
    {
        private readonly List<NetflixPlayback> _allViewedElements;
        private readonly List<NetflixPlayback> _allViewedMovies;
        private readonly List<NetflixPlayback> _allViewedSeriesEpisodes;

        public NetflixStatsCalculator(IEnumerable<NetflixPlayback> netflixPlaybacks)
        {
            _allViewedElements = netflixPlaybacks.ToList();
            _allViewedMovies = GetAllMoviesFromViewedElements(_allViewedElements);
            _allViewedSeriesEpisodes = GetAllSeriesEpisodesFromViewedElements(_allViewedElements);
        }

        public decimal GetTotalViewedMinutes() => _allViewedElements.Sum(x => x.PlaybackDuration) / 60m;

        public decimal GetMoviesViewedMinutes() => _allViewedMovies.Sum(x => x.PlaybackDuration) / 60m;

        public decimal GetSeriesEpisodesViewedMinutes() => _allViewedSeriesEpisodes.Sum(x => x.PlaybackDuration) / 60m;

        public NetflixPlayback GetFirstWatchedMovie() => _allViewedMovies.OrderBy(x => x.PlaybackDateTime).First();

        public NetflixPlayback GetFirstWatchedSeriesEpisode() => _allViewedSeriesEpisodes.OrderBy(x => x.PlaybackDateTime).First();

        public int GetMoviesViewedCount() => _allViewedMovies.Count;

        public int GetSeriesEpisodesViewedCount() => _allViewedSeriesEpisodes.Count;

        public IEnumerable<IGrouping<NetflixSerie, NetflixPlayback>> GetPlaybacksPerSerie() => _allViewedElements.GroupBy(x => x.Episode.Serie);

        public IEnumerable<IGrouping<DateTime, NetflixPlayback>> GetPlaybacksPerDay() => _allViewedElements.GroupBy(x => x.PlaybackDateTime.Date);

        public IDictionary<DateTime, decimal> GetViewedMinutesPerDay()
        {
            var dict = new Dictionary<DateTime, decimal>();
            foreach (var grouping in GetPlaybacksPerDay())
            {
                dict.Add(grouping.Key, grouping.Sum(x => x.PlaybackDuration) / 60m);
            }

            return dict;
        }

        public (decimal minutes, DateTime date) GetHighscoreDateAndMinutes()
        {
            var viewedMinutesPerDay = GetViewedMinutesPerDay();
            var maxValue = viewedMinutesPerDay.Max(x => x.Value);
            return (maxValue, viewedMinutesPerDay.First(x => x.Value == maxValue).Key);
        }

        public IDictionary<NetflixSerie, decimal> GetViewedMinutesPerSerie()
        {
            var dict = new Dictionary<NetflixSerie, decimal>();
            foreach (var grouping in GetPlaybacksPerSerie())
            {
                dict.Add(grouping.Key, grouping.Sum(x => x.PlaybackDuration) / 60m);
            }

            return dict;
        }


        private static List<NetflixPlayback> GetAllMoviesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.Episode.EpisodeType == Enums.EpisodeType.Movie).ToList();

        private static List<NetflixPlayback> GetAllSeriesEpisodesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.Episode.EpisodeType == Enums.EpisodeType.RegularEpisode).ToList();
    }
}
