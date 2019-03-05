namespace NetflixStatizier.Stats.Interface
{
    public interface IStatsCalculator<TPlayback, TSerie, TEpisode> 
        where TSerie : ISerie 
        where TEpisode : IEpisode<TSerie>
    {
        decimal GetTotalViewedHours();

        decimal GetMoviesViewedHours();

        decimal GetSeriesEpisodesViewedHours();

        TPlayback GetFirstWatchedMovie();

        TPlayback GetFirstWatchedSeriesEpisode();

        int GetMoviesViewedCount();

        int GetSeriesEpisodesViewedCount();

    }
}
