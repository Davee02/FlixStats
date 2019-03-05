using System.Collections.Generic;
using System.Linq;
using NetflixStatizier.Stats.Interface;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Stats
{
    public class NetflixStatsCalculator : IStatsCalculator<NetflixPlayback, NetflixSerie, NetflixEpisode>
    {
        private const decimal SECONDS_PER_HOUR = 60 * 60;
        private readonly List<NetflixPlayback> m_AllViewedElements;
        private readonly List<NetflixPlayback> m_AllViewedMovies;
        private readonly List<NetflixPlayback> m_AllViewedSeriesEpisodes;

        public NetflixStatsCalculator(IEnumerable<NetflixPlayback> netflixPlaybacks)
        {
            m_AllViewedElements = netflixPlaybacks.ToList();
            m_AllViewedMovies = GetAllMoviesFromViewedElements(m_AllViewedElements);
            m_AllViewedSeriesEpisodes = GetAllSeriesEpisodesFromViewedElements(m_AllViewedElements);
        }

        public decimal GetTotalViewedHours() => m_AllViewedElements.Sum(x => x.PlaybackDuration) / SECONDS_PER_HOUR;

        public decimal GetMoviesViewedHours() => m_AllViewedMovies.Sum(x => x.PlaybackDuration) / SECONDS_PER_HOUR;

        public decimal GetSeriesEpisodesViewedHours() => m_AllViewedSeriesEpisodes.Sum(x => x.PlaybackDuration) / SECONDS_PER_HOUR;

        public NetflixPlayback GetFirstWatchedMovie() => m_AllViewedMovies.OrderBy(x => x.PlaybackDateTime).First();

        public NetflixPlayback GetFirstWatchedSeriesEpisode() => m_AllViewedSeriesEpisodes.OrderBy(x => x.PlaybackDateTime).First();

        public int GetMoviesViewedCount() => m_AllViewedMovies.Count;

        public int GetSeriesEpisodesViewedCount() => m_AllViewedSeriesEpisodes.Count;


        private static List<NetflixPlayback> GetAllMoviesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.Episode.EpisodeType == Enums.EpisodeType.Movie).ToList();

        private static List<NetflixPlayback> GetAllSeriesEpisodesFromViewedElements(IEnumerable<NetflixPlayback> allViewedItems) =>
            allViewedItems.Where(x => x.Episode.EpisodeType == Enums.EpisodeType.RegularEpisode).ToList();
    }
}
