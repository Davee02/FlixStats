using System;
using System.Globalization;
using NetflixStatizier.Stats.Interface;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixPlayback : IPlayback<NetflixEpisode, NetflixSerie>
    {
        public Enums.DeviceType PlaybackDevice { get; set; }

        public string PlaybackCountry { get; set; }

        public NetflixEpisode Episode { get; set; }

        public DateTime PlaybackDateTime { get; set; }

        public int Sort { get; set; }

        public int PlaybackDuration { get; set; }


        public int CompareTo(IPlayback<IEpisode<ISerie>, ISerie> other)
        {
            return other.Sort.CompareTo(this.Sort);
        }
    }
}
