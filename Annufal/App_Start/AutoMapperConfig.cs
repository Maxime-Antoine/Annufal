using Annufal.Core.Profile;
using AutoMapper;

namespace Annufal
{
    public class AutoMapperConfig
    {
        public static void Config()
        {
            Mapper.CreateMap<CreateProfileBindingModel, ProfileModel>();
        }
    }
}