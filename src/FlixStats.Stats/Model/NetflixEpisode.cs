namespace FlixStats.Stats.Model
{
    public class NetflixEpisode
    {
        public string Title { get; set; }

        public int Sort { get; set; }

        public int Duration { get; set; }

        public string SeasonDescriptor { get; set; }

        public NetflixSerie Serie { get; set; }

        public Enums.EpisodeType EpisodeType { get; set; }

        public int CompareTo(NetflixEpisode other)
        {
            return other.Sort.CompareTo(this.Sort);
        }
    }
}
