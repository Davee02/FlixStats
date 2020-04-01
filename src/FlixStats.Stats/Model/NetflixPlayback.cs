using System;

namespace FlixStats.Stats.Model
{
    public class NetflixPlayback
    {
        public Enums.DeviceType PlaybackDevice { get; set; }

        public string PlaybackCountry { get; set; }

        public NetflixEpisode Episode { get; set; }

        public DateTime PlaybackDateTime { get; set; }

        public int Sort { get; set; }

        public int PlaybackDuration { get; set; }

        public int CompareTo(NetflixPlayback other)
        {
            return other.Sort.CompareTo(this.Sort);
        }

        public override string ToString()
        {
            return Episode.EpisodeType == Enums.EpisodeType.Movie
                ? Episode.Title
                : $"{Episode.Serie.Title}, {Episode.SeasonDescriptor}: \"{Episode.Title}\"";
        }
    }
}
