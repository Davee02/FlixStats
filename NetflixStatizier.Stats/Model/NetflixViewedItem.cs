using System;
using Newtonsoft.Json;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixViewedItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("videoTitle")]
        public string VideoTitle { get; set; }

        [JsonProperty("movieId")]
        public int MovieId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("bookmark")]
        public int Bookmark { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(Helper.MillisecondsUnixTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("deviceType")]
        public int DeviceType { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("topNodeId")]
        public string TopNodeId { get; set; }

        [JsonProperty("series")]
        public int Series { get; set; }

        [JsonProperty("seriesTitle")]
        public string SeriesTitle { get; set; }

        [JsonProperty("seasonDescriptor")]
        public string SeasonDescriptor { get; set; }

        [JsonProperty("episodeTitle")]
        public string EpisodeTitle { get; set; }

        [JsonProperty("estRating")]
        public string EstRating { get; set; }
    }
}
