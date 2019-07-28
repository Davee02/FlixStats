using DaHo.Library.AspNetCore.Data;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        public PaginatedList<LeaderboardItem> LeaderboardItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string GetCountryFlagUrl(string countryCode) => $"https://www.countryflags.io/{countryCode}/flat/32.png";
    }
}
