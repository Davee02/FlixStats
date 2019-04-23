using System.Collections.Generic;
using System.Threading.Tasks;
using NetflixStatizier.Stats.Model;
using RestEase;

namespace NetflixStatizier.Interfaces
{
    [Header("User-Agent", "TWON")]
    [Header("Content-Type", "application/json")]
    public interface INetflixApi
    {
        [Get("viewingHistory")]
        Task<IEnumerable<NetflixPlayback>> GetNetflixViewingHistory([Body(BodySerializationMethod.Serialized)] NetflixProfile netflixProfile);
    }
}
