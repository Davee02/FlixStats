using System.Collections.Generic;
using System.Threading.Tasks;
using NetflixStatizier.Stats.Model;
using RestEase;

namespace NetflixStatizier.Services
{
    [Header("User-Agent", "TWON")]
    [Header("Content-Type", "application/json")]
    public interface INetflixApi
    {
        [Get("viewingHistory")]
        Task<IEnumerable<NetflixViewedItem>> GetNetflixViewingHistory([Body(BodySerializationMethod.Serialized)] NetflixProfile netflixProfile);
    }
}
