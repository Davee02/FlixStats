using NetflixStatizier.Stats.Interface;

namespace NetflixStatizier.Stats.Model
{
    public class NetflixSerie : ISerie
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
                return Title != null ? Title.GetHashCode() : 0;
            }
        }
    }
}
