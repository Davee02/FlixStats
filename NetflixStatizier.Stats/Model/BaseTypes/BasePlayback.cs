using System;

namespace NetflixStatizier.Stats.Model.BaseTypes
{
    public abstract class BasePlayback<TEpisode, TSerie>
        where TEpisode : BaseEpisode<BaseSerie>
        where TSerie : BaseSerie
    {
        public TEpisode Episode { get; set; }

        public DateTime PlaybackDateTime { get; set; }

        public int Sort { get; set; }

        public int PlaybackDuration { get; set; }


        public int CompareTo(BasePlayback<TEpisode, TSerie> other)
        {
            return other.Sort.CompareTo(this.Sort);
        }
    }
}
