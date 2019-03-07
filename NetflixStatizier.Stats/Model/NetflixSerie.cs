using System;
using NetflixStatizier.Stats.Interface;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixSerie : ISerie, IEquatable<NetflixSerie>
    {
        public int IdentificationNumber { get; set; }

        public string Title { get; set; }

        public bool Equals(ISerie other)
        {
            return string.Equals(Title, other.Title);
        }

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
    }
}
