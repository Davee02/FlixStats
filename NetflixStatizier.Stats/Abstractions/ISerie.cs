using System;

namespace NetflixStatizier.Stats.Abstractions
{
    public interface ISerie : IEquatable<ISerie>
    {
        string Title { get; set; }
    }
}