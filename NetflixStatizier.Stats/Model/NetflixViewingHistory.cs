using System;
using System.Collections.Generic;
using System.Text;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixViewingHistory
    {
        public string CodeName { get; set; }
        public List<NetflixViewedItem> ViewedItems { get; set; }
        public int VhSize { get; set; }
        public int Trkid { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string Tz { get; set; }
    }
}
