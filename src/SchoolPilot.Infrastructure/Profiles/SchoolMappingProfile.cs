

using AutoMapper;
using JetBrains.Annotations;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Entities.Schools;
using SchoolPilot.Infrastructure.AutoMapper;
using SchoolPilot.Infrastructure.Commands.School;
using static SchoolPilot.Infrastructure.Commands.School.CreateSchoolAccount;

namespace SchoolPilot.Infrastructure.Profiles
{
    [UsedImplicitly]
    public class SchoolMappingProfile : Profile
    {
        public SchoolMappingProfile()
        {
            CreateMap<CreateSchoolAccount.SchoolModel, BaseSchool>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId , opt => opt.Ignore())
                .ForMember(dest => dest.IsDisabled, opt => opt.Ignore())
                .IgnoreNotAutoMappedAttributes();

            CreateMap<CreateSchoolAccount.LocationModel, SchoolLocation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.SchoolId, opt => opt.Ignore())
                .ForMember(dest => dest.School, opt => opt.Ignore())
                .ForMember(dest => dest.IsDisabled, opt => opt.Ignore())
                .IgnoreNotAutoMappedAttributes();

            CreateMap<CreateSchoolAccount.AddressModelWithRequiredFields, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsValidated, opt => opt.Ignore())
                .ForMember(dest => dest.Latitude, src => src.Ignore())
                .ForMember(dest => dest.Longitude, src => src.Ignore());

            CreateMap<CreateSchoolAccount.PhoneNumberModel, SchoolPhoneNumber>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .IgnoreNotAutoMappedAttributes();

            CreateMap<PhoneNumberModel, NewPhoneNumber>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .IgnoreNotAutoMappedAttributes();

        }
    }
}
