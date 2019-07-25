using System;
using System.Collections.Generic;
using FlixStats.Helper;

namespace FlixStats.Models.ViewModels
{
    public class NetflixPlaybacksViewModel
    {
        public IEnumerable<Stats.Model.NetflixPlayback> Playbacks { get; set; }

        public DateTime PlaybackDate { get; set; }

        public Time TotalPlaybackTime { get; set; }
    }
}
