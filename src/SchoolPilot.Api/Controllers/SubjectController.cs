

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Infrastructure.Queries.Subjects;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SchoolPilot.Api.Controllers
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/subjects")]
    [ApiController]
    public class SubjectController : BaseApiController
    {
        private readonly IMediator _mediator;
        public SubjectController(IMediator mediator, IRedisClient redisCache) : base(mediator, redisCache)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetStudentsAsync()
        {
            var request = new GetSubjects.Query
            {
                AccountId = AccountId.GetValueOrDefault(),
            };

            var result = await Mediator.Send(request);
            return Ok(result.Subjects);
        }
    }
}
