using System.Collections.Generic;
using System.Linq;

namespace NetflixStatizier.Stats.Interface
{
    public interface IStatsCalculator<TPlayback, TSerie, TEpisode> 
        where TSerie : ISerie 
        where TEpisode : IEpisode<TSerie>
        where TPlayback : IPlayback<TEpisode, TSerie>
    {
        decimal GetTotalViewedMinutes();

        decimal GetMoviesViewedMinutes();

        decimal GetSeriesEpisodesViewedMinutes();

        TPlayback GetFirstWatchedMovie();

        TPlayback GetFirstWatchedSeriesEpisode();

        int GetMoviesViewedCount();

        int GetSeriesEpisodesViewedCount();

        IEnumerable<IGrouping<TSerie, TPlayback>> GetPlaybacksPerSerie();

        IDictionary<TSerie, decimal> GetViewedMinutesPerSerie();

    }
}
