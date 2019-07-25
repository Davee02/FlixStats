﻿using System;
using FlixStats.Stats.Model;

namespace FlixStats.Stats.Abstractions
{
    public interface IEpisode<TSerie> : IComparable<IEpisode<ISerie>> 
        where TSerie : ISerie
    {
        string Title { get; set; }

        int Sort { get; set; }

        int Duration { get; set; }

        string SeasonDescriptor { get; set; }

        TSerie Serie { get; set; }

        Enums.EpisodeType EpisodeType { get; set; }
    }
}