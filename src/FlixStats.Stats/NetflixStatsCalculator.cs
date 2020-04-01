using System;
using System.Collections.Generic;
using System.Linq;
using FlixStats.Stats.Abstractions;
using FlixStats.Stats.Model;

namespace FlixStats.Stats
{
    public class NetflixStatsCalculator : IStatsCalculator
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

        public NetflixPlayback GetFirstWatchedMovie() => _allViewedMovies.OrderBy(x => x.PlaybackDateTime).FirstOrDefault();

        public NetflixPlayback GetFirstWatchedSeriesEpisode() => _allViewedSeriesEpisodes.OrderBy(x => x.PlaybackDateTime).FirstOrDefault();

        public int GetMoviesViewedCount() => _allViewedMovies.Count;

        public int GetSeriesEpisodesViewedCount() => _allViewedSeriesEpisodes.Count;

        public IEnumerable<IGrouping<NetflixSerie, NetflixPlayback>> GetPlaybacksPerSerie() => _allViewedElements.GroupBy(x => x.Episode.Serie);

        public IEnumerable<IGrouping<DateTime, NetflixPlayback>> GetPlaybacksPerDay() => _allViewedElements.GroupBy(x => x.PlaybackDateTime.Date);

        public IEnumerable<IGrouping<DayOfWeek, NetflixPlayback>> GetPlaybacksPerWeekDay() => _allViewedElements.GroupBy(x => x.PlaybackDateTime.DayOfWeek);

        public IEnumerable<IGrouping<TimeSpan, NetflixPlayback>> GetPlaybacksPerTimeOfDay() => _allViewedElements.GroupBy(x => x.PlaybackDateTime.TimeOfDay);

        public IEnumerable<IGrouping<string, NetflixPlayback>> GetPlaybacksPerCountry() => _allViewedElements.GroupBy(x => x.PlaybackCountry);

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

        public IDictionary<TimeSpan, decimal> GetViewedMinutesPerTimeOfDay()
        {
            var dict = new Dictionary<TimeSpan, decimal>();
            foreach (var grouping in GetPlaybacksPerTimeOfDay())
            {
                dict.Add(grouping.Key, grouping.Sum(x => x.PlaybackDuration) / 60m);
            }

            return dict;
        }

        public IDictionary<DayOfWeek, decimal> GetViewedMinutesPerWeekDay()
        {
            var dict = new Dictionary<DayOfWeek, decimal>();
            foreach (var grouping in GetPlaybacksPerWeekDay())
            {
                dict.Add(grouping.Key, grouping.Sum(x => x.PlaybackDuration) / 60m);
            }

            return dict;
        }

        public IDictionary<string, decimal> GetViewedMinutesPerCountry()
        {
            var dict = new Dictionary<string, decimal>();
            foreach (var grouping in GetPlaybacksPerCountry())
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
