namespace NetflixStatizier.Stats.Model
{
    public class NetflixViewedItem
    {
        public string Title { get; set; }
        public string VideoTitle { get; set; }
        public int MovieId { get; set; }
        public string Country { get; set; }
        public int Bookmark { get; set; }
        public int Duration { get; set; }
        public object Date { get; set; }
        public int DeviceType { get; set; }
        public string DateStr { get; set; }
        public int Index { get; set; }
        public string TopNodeId { get; set; }
        public int Series { get; set; }
        public string SeriesTitle { get; set; }
        public string SeasonDescriptor { get; set; }
        public string EpisodeTitle { get; set; }
        public string EstRating { get; set; }
    }
}
