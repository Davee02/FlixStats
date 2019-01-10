using System;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixPlay
    {
        public string PlayTitle { get; set; }
        public string PlayCountry { get; set; }
        public int PlayBookmarkTime { get; set; }
        public DateTime PlayWatchTime { get; set; }
        public int PlayDeviceType { get; set; }
        public NetflixShow Show { get; set; }
    }
}
