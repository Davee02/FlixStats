using System;
using Newtonsoft.Json;

namespace FlixStats.Stats.Model
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
        public string CountryCode { get; set; }

        [JsonProperty("bookmark")]
        public int PlaybackBookmark { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(Helper.MillisecondsUnixTimeConverter))]
        public DateTime PlaybackDateTime { get; set; }

        [JsonProperty("deviceType")]
        public Enums.DeviceType DeviceType { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("topNodeId")]
        public string TopNodeId { get; set; }

        [JsonProperty("series")]
        public int SeriesId { get; set; }

        [JsonProperty("seriesTitle")]
        public string SeriesTitle { get; set; }

        [JsonProperty("seasonDescriptor")]
        public string SeasonDescriptor { get; set; }

        [JsonProperty("episodeTitle")]
        public string EpisodeTitle { get; set; }

        [JsonProperty("estRating")]
        public int EstRating { get; set; }
    }
}
