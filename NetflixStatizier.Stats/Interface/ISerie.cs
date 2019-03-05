using System;

namespace NetflixStatizier.Stats.Interface
{
    public interface ISerie : IEquatable<ISerie>
    {
        string Title { get; set; }
    }
}