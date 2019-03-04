using System.Collections.Generic;
using System.Linq;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Stats
{
    public class NetflixStatsClaclulator
    {
        private const decimal SECONDS_PER_HOUR = 60 * 60;
        private readonly List<NetflixPlayback> m_AllViewedElements;
        private readonly List<NetflixPlayback> m_AllViewedMovies;
        private readonly List<NetflixPlayback> m_AllViewedSeriesEpisodes;

        public NetflixStatsClaclulator(IEnumerable<NetflixPlayback> netflixPlaybacks)
        {
            m_AllViewedElements = netflixPlaybacks.ToList();
            m_AllViewedMovies = GetAllMoviesFromViewedElements(m_AllViewedElements);
            m_AllViewedSeriesEpisodes = GetAllSeriesEpisodesFromViewedElements(m_AllViewedElements);
        }

        public decimal GetTotalViewedHours() => m_AllViewedElements.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR;

        public decimal GetMoviesViewedHours() => m_AllViewedMovies.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR;

        public decimal GetSeriesEpisodesViewedHours() => m_AllViewedSeriesEpisodes.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR;

        public NetflixViewedItem GetFirstWatchedMovie() => m_AllViewedMovies.OrderBy(x => x.PlaybackDateTime).First();

        public NetflixViewedItem GetFirstWatchedSeriesEpisode() => m_AllViewedSeriesEpisodes.OrderBy(x => x.PlaybackDateTime).First();

        public int GetMoviesViewedCount() => m_AllViewedMovies.Count;

        public int GetSeriesEpisodesViewedCount() => m_AllViewedSeriesEpisodes.Count;


        private static List<NetflixViewedItem> GetAllMoviesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.).ToList();

        private static List<NetflixViewedItem> GetAllSeriesEpisodesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.SeriesId != 0).ToList();
    }
}
