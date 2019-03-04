namespace NetflixStatizier.Stats.Model.BaseTypes
{
    public abstract class BaseSerie
    {
        protected BaseSerie(string title)
        {
            Title = title;
        }

        protected BaseSerie() { }

        public string Title { get; set; }


        public bool Equals(BaseSerie other)
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
