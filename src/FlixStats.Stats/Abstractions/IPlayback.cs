using System;

namespace FlixStats.Stats.Abstractions
{
    public interface IPlayback<TEpisode, TSerie> : IComparable<IPlayback<IEpisode<ISerie>, ISerie>> 
        where TEpisode : IEpisode<TSerie> 
        where TSerie : ISerie
    {
        TEpisode Episode { get; set; }

        DateTime PlaybackDateTime { get; set; }

        int Sort { get; set; }

        int PlaybackDuration { get; set; }
    }
}