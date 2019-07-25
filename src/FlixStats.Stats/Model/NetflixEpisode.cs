using FlixStats.Stats.Abstractions;

namespace FlixStats.Stats.Model
{
    public class NetflixEpisode : IEpisode<NetflixSerie>
    {
        public string Title { get; set; }

        public int Sort { get; set; }

        public int Duration { get; set; }

        public string SeasonDescriptor { get; set; }

        public NetflixSerie Serie { get; set; }

        public Enums.EpisodeType EpisodeType { get; set; }

        public int CompareTo(IEpisode<ISerie> other)
        {
            return other.Sort.CompareTo(this.Sort);
        }
    }
}
