﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FlixStats.Stats.Abstractions
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

        IEnumerable<IGrouping<string, TPlayback>> GetPlaybacksPerCountry();

        IDictionary<TSerie, decimal> GetViewedMinutesPerSerie();

        IDictionary<DateTime, decimal> GetViewedMinutesPerDay();

        IDictionary<string, decimal> GetViewedMinutesPerCountry();

        (decimal minutes, DateTime date) GetHighscoreDateAndMinutes();

    }
}
