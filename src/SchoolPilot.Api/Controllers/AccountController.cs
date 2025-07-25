

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Infrastructure.Queries.Accounts;
using SchoolPilot.Infrastructure.Queries.Schools;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SchoolPilot.Api.Controllers
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/accounts")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="redisCache"></param>
        public AccountController(IMediator mediator, IRedisClient redisCache) : base(mediator, redisCache) 
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get accounts currently bound to the current user. this endpoint does not require account scoping
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageLength"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAccountsAsync([FromQuery] int page, int pageLength)
        {
            //EnsureAuthState();

            if (HttpMethods.IsOptions(Request.Method))
            {
                return Ok();
            }

            var query = new GetAccounts.Query
            {
                UserId = UserId,
                Page = page,
                PageLength = pageLength,
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route("all-default")]
        public async Task<IActionResult> GetSchoolsAsync([FromQuery] Guid accountId)
        {
            var result = await _mediator.Send(new GetAllDefaultSchools.Query
            {
                AccountId = accountId,
                UserId = UserId.GetValueOrDefault()
            });


            return Ok(result);
        }
    }
}
