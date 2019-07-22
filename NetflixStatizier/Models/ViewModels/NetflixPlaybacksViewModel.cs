using NetflixStatizier.Helper;
using System;
using System.Collections.Generic;

namespace NetflixStatizier.Models.ViewModels
{
    public class NetflixPlaybacksViewModel
    {
        public IEnumerable<Stats.Model.NetflixPlayback> Playbacks { get; set; }

        public DateTime PlaybackDate { get; set; }

        public Time TotalPlaybackTime { get; set; }
    }
}
