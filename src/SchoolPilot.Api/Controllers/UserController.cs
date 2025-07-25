

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Infrastructure.Queries.Users;
using System.Net;
using SchoolPilot.Api.Filters;
using StackExchange.Redis.Extensions.Core.Abstractions;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Infrastructure.Commands.Users;

namespace SchoolPilot.Api.Controllers
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/users")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator, IRedisClient redisCache) : base(mediator, redisCache)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{userId}/permissions")]
        [UserPermissionFilter(ResourceType.Categorized, ParentPermission.Users, PermissionActions.Add, PermissionActions.Edit)]
        public async Task<IActionResult> GetUserPermissionsAsync([FromRoute] Guid? userId = null)
        {
            var request = new GetUserRegularPermissions.Query
            {
                AccountId = AccountId.GetValueOrDefault(),
                UserId = userId
            };

            var result = await Mediator.Send(request);
            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpPut]
        [Route("{userId}/permissions")]
        [UserPermissionFilter(ResourceType.Categorized, ParentPermission.Users, PermissionActions.Add, PermissionActions.Edit)]
        public async Task<IActionResult> EditUserPermissionsAsync([FromRoute] Guid userId, [FromBody] UpdatePermissions.Command request)
        {
            request.AccountId = AccountId.GetValueOrDefault();
            request.RequestUserId = UserId.GetValueOrDefault();
            request.UserId = userId;
            var result = await Mediator.Send(request);
            return Ok(result.StatusCode);
        }
    }
}
