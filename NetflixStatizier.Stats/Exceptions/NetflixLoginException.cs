using System;

namespace NetflixStatizier.Stats.Exceptions
{
    [Serializable]
    public class NetflixLoginException : Exception
    {
        public NetflixLoginException()
        {
        }

        public NetflixLoginException(string message)
            : base(message)
        {
        }

        public NetflixLoginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
