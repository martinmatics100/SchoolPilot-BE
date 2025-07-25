

using AutoMapper;
using JetBrains.Annotations;
using SchoolPilot.Infrastructure.Queries.Users;
using SchoolPilot.Infrastructure.Helpers;
using SchoolPilot.Data.Entities;
using SchoolPilot.Infrastructure.Queries.Accounts;

namespace SchoolPilot.Infrastructure.Profiles
{
    [UsedImplicitly]

    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<PermissionModel, GetAllPermissions.Model>();

            CreateMap<UserAffiliation, GetUserAffiliations.Affiliation>();
        }
    }
}
