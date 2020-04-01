using System;
using System.Collections.Generic;
using DaHo.Library.Utilities;

namespace FlixStats.Models.ViewModels
{
    public class NetflixPlaybacksViewModel
    {
        public IEnumerable<Stats.Model.NetflixPlayback> Playbacks { get; set; }

        public DateTime PlaybackDate { get; set; }

        public Time TotalPlaybackTime { get; set; }
    }
}
