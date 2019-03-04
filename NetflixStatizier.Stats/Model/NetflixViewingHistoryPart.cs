using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetflixStatizier.Stats.Model
{
    internal class NetflixViewingHistoryPart
    {
        [JsonProperty("codeName")]
        public string CodeName { get; set; }

        [JsonProperty("viewedItems")]
        public List<NetflixViewedItem> ViewedItems { get; set; }

        [JsonProperty("vhSize")]
        public int VhSize { get; set; }

        [JsonProperty("trkid")]
        public int Trkid { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("tz")]
        public string Tz { get; set; }
    }
}
