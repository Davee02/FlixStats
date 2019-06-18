using System.Collections.Generic;
using NetflixStatizier.Models.ViewModels;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Services.Abstractions
{
    public interface INetflixStatsCreator
    {
        NetflixStatsViewModel GetNetflixStatsViewModel(IEnumerable<NetflixPlayback> viewingHistory);
    }
}
