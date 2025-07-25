

using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;
using NodaTime;
using PhoneNumbers;
using SchoolPilot.Common.Attributes;
using SchoolPilot.Common.Constants;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Common.Helpers;
using SchoolPilot.Common.Interfaces;
using SchoolPilot.Data;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Entities.Logs;
using SchoolPilot.Data.Entities.Schools;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Helpers;
using SchoolPilot.Infrastructure.Attributes;
using SchoolPilot.Infrastructure.Commands.Subjects;
using SchoolPilot.Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using static SchoolPilot.Infrastructure.Commands.School.CreateSchoolAccount;

namespace SchoolPilot.Infrastructure.Commands.School
{
    public static class CreateSchoolAccount
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public class Request : IRequest<Result>
        {
            [Required]
            [MaxLength(100)]
            public string Name { get; set; }

            public string OwnersFirstName { get; set; }

            public string OwnersLastName { get; set; }

            public UserTitleExtension OwnersTitle { get; set; }

            [RequireNotEmpty]
            public List<SchoolModel> Schools { get; set; }

            [Required]
            public UserModel InitialUser {  get; set; }

            public bool IsTestingAgency { get; set; }

            [Required]
            [Range(1, 10)]
            [NotNull]
            public TimeZones? InitialUserTimeZone { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class SchoolModel // Replacing Providers
        {
            [Required]
            [MaxLength(100)]
            public string SchoolName {  set; get; }

            public SchoolType SchoolType { get; set; }

            public SchoolStatus SchoolStatus { get; set; }

            public SchoolCategory SchoolCategory { get; set; }

            public SchoolOwnership SchoolOwnership { get; set; }

            public Common.Enums.SchoolTerms CurrentTerm { get; set; }

            public string ContactPersonEmail { get; set; }

            public PhoneNumberModel ContactPersonPhone { get; set; }

            [RequireNotEmpty]
            public List<LocationModel> SchoolLocation { get; set; }
        }

        [UsedImplicitly]
        public class AddressModelWithRequiredFields
        {
            [MaxLength(100)]
            [Required(ErrorMessage = "Address Line 1 is required")]
            public string AddressLine1 { get; set; }

            [MaxLength(100)]
            public string AddressLine2 { get; set; }

            [MaxLength(100)]
            [Required(ErrorMessage = "City is required")]
            public string City { get; set; }
            [Required]
            [RegexValidationIf(nameof(Country), Countries.Nigeria, "[A-Z]{2}", ErrorMessage = "The field State must have 2 characters")]
            public string State { get; set; }

            [Required(ErrorMessage = "ZIP is required")]
            [MinLength(2, ErrorMessage = "The field ZIP must be a minimum of 2 characters")]
            [RegexValidationIf(nameof(Country), Countries.Nigeria, "(^$)|(^[0-9]{5,9}$)", ErrorMessage = "The field Zip must be a minimum of 5 digits")]
            [StringLengthExcludeEmpty(10)]
            public string ZipCode { get; set; }

            [MaxLength(100)]
            public string County { get; set; }

            [MaxLength(3)]
            [Required(ErrorMessage = "Country is required")]
            public string Country { get; set; }

            public AddressModelWithRequiredFields()
            {
                Country = Countries.Nigeria;
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class PhoneNumberModel
        {
            [MaxLength(14)]
            [RegexValidationIf(nameof(Country), Countries.Nigeria, "^[0-9]{11}$", ErrorMessage = "Enter a valid 11-digit phone number.")]
            public string Number { get; set; }

            [MaxLength(5)]
            public string Extension { get; set; }

            public PhoneType Type { get; set; }

            [MaxLength(3)]
            public string Country { get; set; }

            //Set a default, so that the other addresses that are not setting
            //it yet won't fail due to the field being not nullable in the db.
            public PhoneNumberModel()
            {
                Country = Countries.Nigeria;
            }
        }
        public class UserModel
        {
            [Required(ErrorMessage = "Email is required")]
            [MaxLength(64)]
            public string Email { get; set; }

            [Required]
            [MaxLength(100)]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(100)]
            public string LastName { get; set; }

            public string Password { get; set; }
        }

        public class LocationModel
        {
            [Required]
            [MaxLength(100)]
            public string Name { get; set; }

            [RequireNonDefault]
            public AddressModelWithRequiredFields Address { get; set; }

            public PhoneNumberModel PrimaryPhone { get; set; }

            public TimeZones TimeZone { get; set; }

            public bool IsMainLocation { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }

            public List<SchoolResult> Schools { get; set; }

            public string ErrorMessage { get; set; }
        }

        public class SchoolResult
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public List<LocationResult> Locations { get; set; }

            public SchoolResult()
            {
                Locations = new List<LocationResult>();
            }
        }

        public class LocationResult
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IMapper _mapper;
            private readonly ReadWriteSchoolPilotContext _readWriteContext;
            private readonly ReadSchoolPilotContext _readContext;
            private readonly IPermissionMapper _permissionMapper;
            private readonly IMethodExecutionHelper _executionHelper;
            private readonly IPasswordHasher _passwordHasher;

            public Handler(IMapper mapper, ReadWriteSchoolPilotContext readWriteContext, ReadSchoolPilotContext readContext, 
                IPermissionMapper permissionMapper, IMethodExecutionHelper executionHelper, IPasswordHasher passwordHasher)
            {
                _mapper = mapper;
                _readWriteContext = readWriteContext;
                _readContext = readContext;
                _permissionMapper = permissionMapper;
                _executionHelper = executionHelper;
                _passwordHasher = passwordHasher;
            }

            public async Task<Result> Handle(Request message, CancellationToken cancellationToken)
            {
                var result = new Result();

                var account = new Account
                {
                    Id = SequentialGuid.Create(),
                    Name = message.Name,
                    SchooOwnerFirstName = message.OwnersFirstName,
                    SchoolOwnerLastName = message.OwnersLastName,
                    OwnerTitle = message.OwnersTitle,
                    Flags = message.IsTestingAgency ? AccountMiscFlags.IsTestingAccount : AccountMiscFlags.None
                };

                var schoolResults = new List<SchoolResult>();
                _readWriteContext.Accounts.Add(account);
                var agencyLocations = new List<SchoolLocation>();


                var schoolIds = new List<Guid>();

                if (message.Schools == null || !message.Schools.Any())
                {
                    result.ErrorMessage = "No schools provided";
                    return result;
                }

                foreach (var provider in message.Schools)
                {
                    var schoolProvider = _mapper.Map<BaseSchool>(provider);
                    schoolProvider.AccountId = account.Id;
                    schoolProvider.Id = SequentialGuid.Create();

                    schoolIds.Add(schoolProvider.Id);

                    if (provider.ContactPersonPhone != null)
                    {
                        var contactPersonPhone = new SchoolPhoneNumber
                        {
                            Id = SequentialGuid.Create(),
                            AccountId = account.Id,
                            Number = provider.ContactPersonPhone.Number,
                            Extension = provider.ContactPersonPhone.Extension,
                            Type = provider.ContactPersonPhone.Type,
                        };
                        _readWriteContext.SchoolPhoneNumbers.Add(contactPersonPhone);
                        schoolProvider.ContactPersonPhone = contactPersonPhone;
                        schoolProvider.ContactPersonPhone_Id = contactPersonPhone.Id;
                    }


                    var schoolSettings = new SchoolSetting
                    {
                        AccountId = account.Id,
                        SchoolId = schoolProvider.Id,
                        TasksToBypass = TasksToBypass.None,
                    };

                    _readWriteContext.SchoolSettings.Add(schoolSettings);

                    var locationResults = new List<LocationResult>();

                    foreach (var locationModel in provider.SchoolLocation)
                    {
                        var location = _mapper.Map<SchoolLocation>(locationModel);
                        agencyLocations.Add(location);
                        location.Id = SequentialGuid.Create();
                        location.AccountId = account.Id;
                        location.SchoolId = schoolProvider.Id;
                        location.School = schoolProvider;

                        if(locationModel.Address != null)
                        {
                            var address = new Address
                            {
                                Id = SequentialGuid.Create(),
                                AddressLine1 = locationModel.Address.AddressLine1,
                                AddressLine2 = locationModel.Address.AddressLine2,
                                City = locationModel.Address.City,
                                State = locationModel.Address.State,
                                Country = locationModel.Address.Country,
                                ZipCode = locationModel.Address.ZipCode,
                                County = locationModel.Address.County ?? string.Empty
                            };

                            _readWriteContext.Addresses.Add(address);
                            location.Address = address;
                            location.Address_Id = address.Id;
                        }

                        // Initialize phone number as NewPhoneNumber
                        if (locationModel.PrimaryPhone != null)
                        {
                            var phoneNumber = new NewPhoneNumber
                            {
                                Id = SequentialGuid.Create(),
                                AccountId = account.Id,
                                Number = locationModel.PrimaryPhone.Number,
                                Extension = locationModel.PrimaryPhone.Extension,
                                Type = locationModel.PrimaryPhone.Type,
                            };
                            _readWriteContext.PhoneNumbers.Add(phoneNumber);

                            location.PrimaryPhone = phoneNumber;
                            location.PrimaryPhone_Id = phoneNumber.Id;
                        }


                        _readWriteContext.SchoolLocations.Add(location);

                        locationResults.Add(new LocationResult
                        {
                            Id = location.Id,
                            Name = location.Name
                        });
                    }
                    schoolResults.Add(new SchoolResult
                    {
                        Id = schoolProvider.Id,
                        Name = schoolProvider.SchoolName,
                        Locations = locationResults
                    });

                    _readWriteContext.Schools.Add(schoolProvider);

                    #region Activity Log

                    var activityLog = new ActivityLog
                    {
                        Id = SequentialGuid.Create(),
                        AccountId = account.Id,
                        UserId = Guid.Empty,
                        DomainId = account.Id,
                        DomainType = ActivityDomainType.School,
                        EntityType = ActivityEntityType.Student,
                        EntityId = account.Id,
                        ActionId = (int)ActivityLogStudentAction.CreateStudent,
                        EntityName = account.Name,
                    };

                    _readWriteContext.ActivityLogs.Add(activityLog);

                    #endregion

                    var failureResult = await CreateUser(message, account, agencyLocations);

                    if (failureResult != null)
                    {
                        return failureResult;
                    }
                }

                try
                {
                    if (await _readWriteContext.SaveChangesAsync() > 0)
                    {

                        // Load default subjects for each created school
                        foreach (var schoolId in schoolIds)
                        {
                            _executionHelper.ExecuteOnAnything<LoadDefaultSubjects.Handler>(x => x.Handle(
                                new LoadDefaultSubjects.Command
                                {
                                    AccountId = account.Id
                                },
                                cancellationToken
                            ));
                        }


                        // Get the created user ID (from the newly created user)
                        var userId = (await _readWriteContext.Users
                                        .Where(u => u.Email == message.InitialUser.Email)
                                        .Select(u => u.Id)
                                        .FirstOrDefaultAsync());

                        if (userId == Guid.Empty)
                        {
                            Logger.Error($"Failed to find created user with email: {message.InitialUser.Email}");
                            result.ErrorMessage = "Could not locate created user";
                            return result;
                        }

                        // Create list of default schools from the newly created schools
                        var defaultSchools = schoolResults.Select(s => new UpsertDefaultSchools.DefaultSchoolModel
                        {
                            Id = s.Id,
                            IsDefault = true
                        }).ToList();

                        //Create default general ledger accounts for each location
                        Logger.Debug($"Attempting to create or update default Schools for {account.Name}. Id: {account.Id}");
                        _executionHelper.ExecuteOnAnything<UpsertDefaultSchools.Handler>(x => x.Handle(new UpsertDefaultSchools.Command
                        {
                            AccountId = account.Id,
                            UserId = userId,
                            DefaultSchools = defaultSchools

                        }, cancellationToken));

                        result.Id = account.Id;
                        result.Schools = schoolResults;
                        return result;
                    }
                }
                catch (DbEntityValidationException e)
                {
                    Logger.Error(e, e.Format());
                }
                catch (DbUpdateException e)
                {
                    result.ErrorMessage = e.Format();
                    Logger.Error(e, result.ErrorMessage);
                    return result;
                }

                result.ErrorMessage = "Could not create School Account";
                return result;
            }
            private async Task<Result> CreateUser(Request message, Account agency, List<SchoolLocation> agencyLocations)
            {
                var userId = await (from user in _readWriteContext.Users
                                    where user.Email == message.InitialUser.Email
                                    select user.Id).SingleOrDefaultAsync();

                if(userId != default(Guid))
                {
                    var userAffExists = (from aff in _readWriteContext.UserAffiliations
                                         where aff.User.Id == userId
                                         && aff.Account.Id == agency.Id
                                         && !aff.IsDeprecated
                                         select aff).Any();

                    if (userAffExists)
                    {
                        throw new DbUpdateException("UserAffiliation already exists");
                    }
                }

                if(userId == default(Guid))
                {
                    var hashedPassword = _passwordHasher.HashPassword(message.InitialUser.Password);

                    var newUserId = SequentialGuid.Create();

                    var user = new User
                    {
                        LoginId = newUserId,
                        Email = message.InitialUser.Email,
                        FirstName = message.InitialUser.FirstName,
                        LastName = message.InitialUser.LastName,
                        Password = hashedPassword,
                        Role = UserRole.Admin,
                        InviteId = string.Empty,
                        IsDeprecated = false,
                        Id = SequentialGuid.Create(),
                    };

                    _readWriteContext.Users.Add(user);
                    userId = user.Id;
                }

                var userAffiliation = new UserAffiliation
                {
                    Id = SequentialGuid.Create(),
                    AccountId = agency.Id,
                    UserId = userId,
                    FirstName = message.InitialUser.FirstName,
                    LastName = message.InitialUser.LastName,
                    Email = message.InitialUser.Email,
                    Status = UserStatus.Active,
                    Role = UserRole.Admin,
                    HomePhone = new UserAffiliationPhoneNumber
                    {
                        AccountId = agency.Id,
                        Extension = string.Empty,
                        Number = string.Empty,
                        Type = PhoneType.Home
                    },
                    MobilePhone = new UserAffiliationPhoneNumber
                    {
                        AccountId = agency.Id,
                        Extension = string.Empty,
                        Number = string.Empty,
                        Type = PhoneType.Mobile
                    }
                };

                _readWriteContext.UserAffiliations.Add(userAffiliation);

                AddPermissions(agency, userAffiliation);

                var timeAttribute = message.InitialUserTimeZone.Value.GetAttribute<TimeZones, TimeZoneAttribute>();
                var userTimeZone = timeAttribute.GetTimeZone();
                var today = userTimeZone.GetNow();
                var userAffiliationHistory = new UserAffiliationHistory
                {
                    AccountId = agency.Id,
                    UserAffiliationId = userAffiliation.Id,
                    Status = UserStatus.Active,
                    StartDate = today,
                    Comments = "Create Account"
                };

                _readWriteContext.UserAffiliationsHistories.Add(userAffiliationHistory);
                foreach (var location in agencyLocations)
                {
                    _readWriteContext.UserLocations.Add(new UserLocation
                    {
                        AccountId = agency.Id,
                        AgencyLocationId = location.Id,
                        UserId = userId
                    });
                }

                return null;
            }

            private void AddPermissions(Account account, UserAffiliation userAffiliation)
            {
                var principle = new Principle
                {
                    AccountId = account.Id,
                    Id = SequentialGuid.Create()
                };

                var permissions = _permissionMapper.GetPermissionList()
                    .Select(s => new Permission
                    {
                        AccountId = account.Id,
                        Action = (int)s.Action,
                        Resource = s.Resource,
                        ResourceType = (int)s.ResourceType,
                        Value = 0,
                        PrincipleId = principle.Id,

                        //Distinct shouldn't be necessary, but helps ensure the unique constraint in the table is not triggered

                    })
                    .Distinct(new DistinctPermissionComparer())
                    .ToList();

                _readWriteContext.Principles.Add(principle);
                _readWriteContext.Permissions.AddRange(permissions);
                _readWriteContext.UserPrinciples.Add(new UserPrinciple
                {
                    PrincipleId = principle.Id,
                    AccountId = account.Id,
                    UserAffiliationId = userAffiliation.Id,
                    ExpirationDate = DateTimeExtensions.MaxDate,
                    IsActive = true,
                });

                var readOnlyPrinciple = new Principle
                {
                    AccountId = account.Id,
                    Id = SequentialGuid.Create(),
                    PermissionType = PermissionAccessType.Readonly
                };

                var readonlyActions = Enum.GetValues(typeof(ReadonlyPermissionActions)).Cast<PermissionActions>();

                var readonlyPermissions = _permissionMapper.GetPermissionList()
                    .Select(s => new Permission
                    {
                        AccountId = account.Id,
                        Action = (int)s.Action,
                        Resource = s.Resource,
                        ResourceType = (int)s.ResourceType,
                        Value = readonlyActions.Contains(s.Action) ? PermissionValue.Granted : PermissionValue.Denied,
                        PrincipleId = readOnlyPrinciple.Id,

                        //Distinct shouldn't be necessary, but helps ensure the unique constraint in the table is not triggered
                    })
                    .Distinct(new DistinctPermissionComparer())
                    .ToList();

                _readWriteContext.Principles.Add(readOnlyPrinciple);
                _readWriteContext.Permissions.AddRange(readonlyPermissions);
            }
        }
    }

    internal class DistinctPermissionComparer : IEqualityComparer<Permission>
    {
        public bool Equals(Permission x, Permission y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.PrincipleId == y.PrincipleId && x.Resource == y.Resource && x.ResourceType == y.ResourceType && x.Action == y.Action;
        }

        public int GetHashCode(Permission obj)
        {
            return obj.PrincipleId.GetHashCode() ^ (obj.Resource.GetHashCode() * obj.ResourceType.GetHashCode() + obj.Action.GetHashCode());
        }
    }

}
