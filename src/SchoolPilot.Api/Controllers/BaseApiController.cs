

using MediatR;
using StackExchange.Redis.Extensions.Core.Abstractions;
using SchoolPilot.Api.Interfaces;
using SchoolPilot.Api.Models;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Api.Filters;
using SchoolPilot.Common.Attributes;
using NodaTime;
using SchoolPilot.Common.Enums;
using System.Net;
using SchoolPilot.Infrastructure.Queries.Accounts;
using SchoolPilot.Infrastructure.Interfaces;
using SchoolPilot.Infrastructure.Commands.Assets;
using SchoolPilot.Business.BasicResults;
using SchoolPilot.Infrastructure.BasicResults;

namespace SchoolPilot.Api.Controllers
{

    public interface IBaseApiController : ITimeZoneApiController
    {
        Guid? AccountId { get; set; }

        Guid? UserId { get; set; }

        IMediator Mediator { get; } // HACK for filters

        IRedisClient RedisCache { get; } // HACK for filters

        CurrentUser CurrentUser { get; set; }

        Task<Guid> GetSingleUserAffiliationId();

        Task<Guid> GetSingleAccountId();
    }

    [TypeFilter(typeof(UserAuthorizeFilterAttribute))]
    [TypeFilter(typeof(AccountScopeFilterAttribute))]
    [TypeFilter(typeof(TimeZoneFilterAttribute))]
    public abstract class BaseApiController : ControllerBase, IBaseApiController
    {
        private Guid _userAffiliationId;

        public BaseApiController(IMediator mediator, IRedisClient redisCache)
        {
            Mediator = mediator;
            RedisCache = redisCache;
        }

        public IMediator Mediator { get; }

        public IRedisClient RedisCache { get; }

        public Guid? AccountId { get; set; }

        public Guid? UserId { get; set; }

        public bool IsSupportUser { get; set; }

        public bool AllowEmergencyAccessRequest { get; set; }

        public CurrentUser CurrentUser { get; set; }

        //This is essentially the old implementation of FilterableByLocation, but moved here
        //to accomodate the readonly mode. This way only readonly or nonreadonly locations
        //are guaranteed in all endpoints that need it
        public List<Guid> DefaultLocationIds { get; set; }

        //Time zone is handled as 2 properties, so that we only parse the string into a timezone when we need it.
        // Originally the get was private, but am making the get public in order to retrieve the string for exports.
        public string TimeZoneString { get; set; }

        public DateTimeZone TimeZone => DateTimeZoneProviders.Tzdb.GetZoneOrNull(TimeZoneString);

        [NonAction]
        public async Task<Guid> GetSingleAccountId()
        {
            await GetSingleUserAffiliationId();
            if (AccountId == null)
            {
                throw new Exception("This state should be impossible, check if affiliations are handled correctly on this request.");
            }

            return AccountId.Value;
        }

        [NonAction]
        public async Task<Guid> GetSingleUserAffiliationId()
        {
            if (_userAffiliationId != default(Guid))
            {
                return _userAffiliationId;
            }

            if (UserId == null)
            {
                throw new HttpRequestException("No User available on this request, ensure that the user is authorized", null, HttpStatusCode.Unauthorized);
            }

            var result = await Mediator.Send(new GetUserAffiliations.Query
            {
                AccountId = AccountId,
                UserId = UserId
            });

            var affiliationsCount = result.Affiliations.Count;
            if (affiliationsCount > 1)
            {
                throw new HttpRequestException(
                    "Multiple user affliations are available on this request, which is not supported. Consider declaring affliation scope via the X-Account-Id header.",
                    null,
                    HttpStatusCode.BadRequest);
            }

            if (affiliationsCount < 1)
            {
                throw new HttpRequestException(
                    "No user affliations available on this request, ensure that the user is authorized and that the account scope is correct via the X-Account-Id header.",
                    null,
                    HttpStatusCode.Forbidden);
            }

            var affiliation = result.Affiliations.Single();
            if (affiliation.Status == UserStatus.Inactive)
            {
                throw new HttpRequestException(
                    "No user affliations available on this request, ensure that the user is authorized and that the account scope is correct via the X-Account-Id header.",
                    null,
                    HttpStatusCode.Forbidden);
            }

            AccountId = affiliation.AccountId;
            IsSupportUser = affiliation.IsSupportUser;
            AllowEmergencyAccessRequest = affiliation.AllowEmergencyAccessRequest;

            var locationInformation = await Mediator.Send(new GetUserDefaultLocationInformation.Query
            {
                AccountId = AccountId.Value,
                UserId = UserId.Value
            });
            DefaultLocationIds = locationInformation.LocationIds;

            if (locationInformation.IsInReadonlyMode && !affiliation.AllowReadOnlyAccess)
            {
                throw new HttpRequestException(
                   "User does not have access to read only mode and is trying to access read only data.",
                   null,
                   HttpStatusCode.Forbidden);
            }



            return _userAffiliationId = result.Affiliations.Single().Id;
        }

        [NonAction]
        public async Task<bool> HandleFileUpload<T>(T command) where T : IHasAsset
        {
            if (string.IsNullOrWhiteSpace(command.FileName) || command.FileData == null)
            {
                return false;
            }

            using var stream = new MemoryStream(command.FileData);

            var uploadAssetCommand = new UploadAsset.Command
            {
                AccountId = AccountId.GetValueOrDefault(),
                FileName = command.FileName,
                FileStream = stream
            };

            var assetInformation = await Mediator.Send(uploadAssetCommand);

            if (string.IsNullOrWhiteSpace(assetInformation.Url) || assetInformation.FileId == Guid.Empty)
            {
                return false;
            }

            command.AssetToken = assetInformation.Url; // Now storing URL instead of token
            command.AssetId = assetInformation.FileId;

            return true;
        }


        [NonAction]
        public Microsoft.AspNetCore.Mvc.IActionResult Respond(Business.BasicResults.IActionResult result)
        {
            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result);
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return StatusCode((int)result.Status, result.ErrorMessage);
            }

            return StatusCode((int)result.Status);
        }

        [NonAction]
        public Microsoft.AspNetCore.Mvc.IActionResult Respond(IValidationResult result)
        {
            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result);
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return StatusCode((int)result.Status, new
                {
                    result.ErrorMessage,
                    result.Errors
                });
            }

            return StatusCode((int)result.Status);
        }

        [NonAction]
        protected void EnsureAuthState()
        {
            if (HttpContext.Items.TryGetValue("AuthData", out var authDataObj) &&
                authDataObj is AccountScopeFilterAttribute.AuthData authData)
            {
                UserId = authData.UserId ?? UserId;
                CurrentUser = authData.CurrentUser ?? CurrentUser;
                AccountId = authData.AccountId ?? AccountId;
            }
        }

    }
}
