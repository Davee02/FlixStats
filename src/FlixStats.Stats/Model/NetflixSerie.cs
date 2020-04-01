using System;

namespace FlixStats.Stats.Model
{
    public class NetflixSerie : IEquatable<NetflixSerie>
    {
        public int IdentificationNumber { get; set; }

        public string Title { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IdentificationNumber * 397) ^ (Title != null ? Title.GetHashCode() : 0);
            }
        }

        public bool Equals(NetflixSerie other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IdentificationNumber == other.IdentificationNumber
                   && string.Equals(Title, other.Title);
        }

        public override string ToString() => Title;
    }
}
