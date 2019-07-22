using AutoMapper;

namespace NetflixStatizier.Models.ModelMapper
{
    public class NetflixViewedItemProfile : Profile
    {
        public NetflixViewedItemProfile()
        {
            CreateMap<Stats.Model.NetflixViewedItem, Models.EntityFrameworkModels.NetflixViewedItem>();

            CreateMap<Models.EntityFrameworkModels.NetflixViewedItem, Stats.Model.NetflixViewedItem>();

            CreateMap<Models.EntityFrameworkModels.NetflixViewedItem, Models.ImportExportModels.NetflixViewedItem>();

        }
    }
}
