using System;

namespace FlixStats.Stats.Abstractions
{
    public interface ISerie : IEquatable<ISerie>
    {
        string Title { get; set; }
    }
}