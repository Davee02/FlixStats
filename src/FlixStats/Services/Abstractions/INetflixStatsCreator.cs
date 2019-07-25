using System.Collections.Generic;
using FlixStats.Models.ViewModels;
using FlixStats.Stats.Model;

namespace FlixStats.Services.Abstractions
{
    public interface INetflixStatsCreator
    {
        NetflixStatsViewModel GetNetflixStatsViewModel(IEnumerable<NetflixPlayback> viewingHistory);

        NetflixPlaybacksViewModel GetNetflixPlaybacksViewModel(IEnumerable<NetflixPlayback> viewingHistory);
    }
}
