using System.Collections.Generic;
using System.Linq;
using NetflixStatizier.Stats.Interface;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Stats
{
    public class NetflixStatsCalculator : IStatsCalculator<NetflixPlayback, NetflixSerie, NetflixEpisode>
    {
        private readonly List<NetflixPlayback> m_AllViewedElements;
        private readonly List<NetflixPlayback> m_AllViewedMovies;
        private readonly List<NetflixPlayback> m_AllViewedSeriesEpisodes;

        public NetflixStatsCalculator(IEnumerable<NetflixPlayback> netflixPlaybacks)
        {
            m_AllViewedElements = netflixPlaybacks.ToList();
            m_AllViewedMovies = GetAllMoviesFromViewedElements(m_AllViewedElements);
            m_AllViewedSeriesEpisodes = GetAllSeriesEpisodesFromViewedElements(m_AllViewedElements);
        }

        public decimal GetTotalViewedMinutes() => m_AllViewedElements.Sum(x => x.PlaybackDuration) / 60m;

        public decimal GetMoviesViewedMinutes() => m_AllViewedMovies.Sum(x => x.PlaybackDuration) / 60m;

        public decimal GetSeriesEpisodesViewedMinutes() => m_AllViewedSeriesEpisodes.Sum(x => x.PlaybackDuration) / 60m;

        public NetflixPlayback GetFirstWatchedMovie() => m_AllViewedMovies.OrderBy(x => x.PlaybackDateTime).First();

        public NetflixPlayback GetFirstWatchedSeriesEpisode() => m_AllViewedSeriesEpisodes.OrderBy(x => x.PlaybackDateTime).First();

        public int GetMoviesViewedCount() => m_AllViewedMovies.Count;

        public int GetSeriesEpisodesViewedCount() => m_AllViewedSeriesEpisodes.Count;

        public IEnumerable<IGrouping<NetflixSerie, NetflixPlayback>> GetPlaybacksPerSerie() => m_AllViewedElements.GroupBy(x => x.Episode.Serie);

        public IDictionary<NetflixSerie, decimal> GetViewedMinutesPerSerie()
        {
            IDictionary<NetflixSerie, decimal> dict = new Dictionary<NetflixSerie, decimal>();
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
