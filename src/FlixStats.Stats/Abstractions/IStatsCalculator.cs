using FlixStats.Stats.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlixStats.Stats.Abstractions
{
    public interface IStatsCalculator
    {
        decimal GetTotalViewedMinutes();

        decimal GetMoviesViewedMinutes();

        decimal GetSeriesEpisodesViewedMinutes();

        NetflixPlayback GetFirstWatchedMovie();

        NetflixPlayback GetFirstWatchedSeriesEpisode();

        int GetMoviesViewedCount();

        int GetSeriesEpisodesViewedCount();

        IEnumerable<IGrouping<NetflixSerie, NetflixPlayback>> GetPlaybacksPerSerie();

        IEnumerable<IGrouping<DateTime, NetflixPlayback>> GetPlaybacksPerDay();

        IEnumerable<IGrouping<string, NetflixPlayback>> GetPlaybacksPerCountry();

        IDictionary<NetflixSerie, decimal> GetViewedMinutesPerSerie();

        IDictionary<DateTime, decimal> GetViewedMinutesPerDay();

        IDictionary<string, decimal> GetViewedMinutesPerCountry();

        (decimal minutes, DateTime date) GetHighscoreDateAndMinutes();

    }
}
