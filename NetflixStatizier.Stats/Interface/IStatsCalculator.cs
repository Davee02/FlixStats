using System;
using System.Collections.Generic;
using System.Linq;

namespace NetflixStatizier.Stats.Interface
{
    public interface IStatsCalculator<out TPlayback, TSerie, TEpisode> 
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

        IEnumerable<IGrouping<DateTime, TPlayback>> GetPlaybacksPerDay();

        IDictionary<TSerie, decimal> GetViewedMinutesPerSerie();

        IDictionary<DateTime, decimal> GetViewedMinutesPerDay();

    }
}
