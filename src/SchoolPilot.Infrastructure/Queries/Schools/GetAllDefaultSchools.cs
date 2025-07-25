

using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolPilot.Data.Context;
using static SchoolPilot.Infrastructure.Commands.School.CreateSchoolAccount;

namespace SchoolPilot.Infrastructure.Queries.Schools
{
    public static class GetAllDefaultSchools
    {
        public class Query : IRequest<Result>
        {
            [JsonIgnore]
            public Guid? AccountId { get; set; }

            [JsonIgnore]
            public Guid? UserId { get; set; }
        }

        public class Result
        {
            public Result()
            {
                RegularSchools = new List<DefaultSchoolModel>();
                ReadonlySchools = new List<DefaultSchoolModel>();
            }

            public List<DefaultSchoolModel> RegularSchools { get; set; }

            //this does not mean the entire school is readonly, just that some locations in it are
            public List<DefaultSchoolModel> ReadonlySchools { get; set; }
        }

        public class DefaultSchoolModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public bool IsDefault { get; set; }

            public List<LocationModel> Locations { get; set; }
        }

        public class LocationModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;

            private readonly IMapper _mapper;

            public Handler(ReadSchoolPilotContext readContext, IMapper mapper)
            {
                _readContext = readContext;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = new Result();

                var schoolsAndLocationsInfo = await (from userAff in _readContext.UserAffiliations
                                                     join userLocation in _readContext.UserLocations on userAff.UserId equals userLocation.UserId
                                                     join schoolLocation in _readContext.SchoolLocations on userLocation.AgencyLocationId equals schoolLocation.Id
                                                     join school in _readContext.Schools on schoolLocation.SchoolId equals school.Id
                                                     where userLocation.UserId == message.UserId
                                                     && userAff.AccountId == message.AccountId
                                                     && userLocation.AccountId == message.AccountId
                                                     && !schoolLocation.IsDeprecated
                                                     && !schoolLocation.IsDisabled
                                                     && !school.IsDeprecated
                                                     && !school.IsDisabled
                                                     && (!schoolLocation.IsReadOnly
                                                     || userAff.AllowReadOnlyAccess && schoolLocation.IsReadOnly)
                                                     select new
                                                     {
                                                         schoolLocation.SchoolId,
                                                         schoolLocation.IsReadOnly,
                                                         schoolLocation.Id,
                                                         schoolLocation.Name,
                                                     }).ToListAsync();

                var schoolIds = schoolsAndLocationsInfo.Select(x => x.SchoolId).Distinct().ToList();

                var schoolDictionary = schoolsAndLocationsInfo.GroupBy(x => (x.SchoolId, x.IsReadOnly))
                        .ToDictionary(x => x.Key, k => k.Select(loc => new LocationModel
                        {
                            Id = loc.Id,
                            Name = loc.Name,
                        }).ToList());

                var schools = await (from school in _readContext.Schools
                                     join defaultSchool in _readContext.UserDefaultSchools.Where(d => d.UserId == message.UserId && d.AccountId == message.AccountId) on school.Id equals
                                         defaultSchool.SchoolId into defaultSchools
                                     from defaultSchool in defaultSchools.DefaultIfEmpty()
                                     orderby school.SchoolName
                                     where schoolIds.Contains(school.Id)
                                           && school.AccountId == message.AccountId
                                           && !school.IsDeprecated
                                           && !school.IsDisabled
                                     select new DefaultSchoolModel
                                     {
                                         Id = school.Id,
                                         Name = school.SchoolName,
                                         IsDefault = defaultSchool != null
                                     }).ToListAsync();

                foreach (var school in schools)
                {
                    if (schoolDictionary.ContainsKey((school.Id, true)))
                    {
                        school.Locations = schoolDictionary[(school.Id, true)];
                        result.ReadonlySchools.Add(school);
                    }
                    if (schoolDictionary.ContainsKey((school.Id, false)))
                    {
                        school.Locations = schoolDictionary[(school.Id, false)];
                        result.RegularSchools.Add(school);

                    }
                }

                return result;
            }
        }
    }
}
