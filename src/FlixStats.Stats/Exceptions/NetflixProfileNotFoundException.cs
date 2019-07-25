using System;

namespace FlixStats.Stats.Exceptions
{
    [Serializable]
    public class NetflixProfileNotFoundException : NetflixLoginException
    {
        public NetflixProfileNotFoundException()
        {
        }

        public NetflixProfileNotFoundException(string message)
            : base(message)
        {
        }

        public NetflixProfileNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public string ProfileName { get; set; }
    }
}
