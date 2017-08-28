using AdSale.Models;
using AdSale.ServiceModels;
using AutoMapper;

namespace AdSale.Profiles
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
            : this("AdSaleProfile")
        {
        }
        protected AutoMapperProfileConfiguration(string profileName)
            : base(profileName)
        {
            CreateMap<Media, MediaModel>();
            CreateMap<Category, CategoryModel>();
            CreateMap<AdType, AdTypeModel>();
            CreateMap<AdTypeModel, AdType>();
        }
    }
}
