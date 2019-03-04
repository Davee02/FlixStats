namespace NetflixStatizier.Stats.Model.BaseTypes
{
    public abstract class BaseEpisode<TSerie> where TSerie : BaseSerie
    {
        protected BaseEpisode(string title, int sort = 0)
        {
            Title = title;
            Sort = sort;
        }

        protected BaseEpisode() { }


        public string Title { get; set; }

        public int Sort { get; set; }

        public int Duration { get; set; }

        public string SeasonDescriptor { get; set; }

        public TSerie Serie { get; set; }

        public Enums.EpisodeType EpisodeType { get; set; }

        public int CompareTo(BaseEpisode<TSerie> other)
        {
            return other.Sort.CompareTo(this.Sort);
        }
    }
}
