

using AutoMapper;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Infrastructure.Queries.CustomEnums;

namespace SchoolPilot.Infrastructure.Profiles
{
    public class EnumMappingProfile : Profile
    {
        public EnumMappingProfile()
        {
            CreateMap<EnumExtensions.EnumValueModel, GetPhoneNumberType.Model>()
                .ForMember(dest => dest.Value, src => src.MapFrom(opt => opt.Value))
                .ForMember(src => src.HasExtension, opt => opt.Ignore());
        }
    }
}
