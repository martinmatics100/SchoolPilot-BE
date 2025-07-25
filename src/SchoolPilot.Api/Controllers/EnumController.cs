

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Common.Helpers;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SchoolPilot.Api.Controllers
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/enums")]
    [ApiController]
    public class EnumController : BaseApiController
    {
        private readonly IEnumEngine _enumEngine;

        public EnumController(IMediator mediator, IRedisClient redisCache, IEnumEngine enumEngine) : base(mediator, redisCache)
        {
            _enumEngine = enumEngine;
        }


        [HttpGet]
        [Route("{enumName}")]
        public IActionResult GenericEnum(string enumName)
        {
            EnsureAuthState();
            return Ok(_enumEngine.GetValues(enumName));
        }

        [HttpGet]
        [Route("")]
        public IActionResult EnumEngineNames()
        {
            EnsureAuthState();
            return Ok(_enumEngine.GetNames());
        }

        [HttpGet]
        [Route("genders")]
        public IActionResult Genders()
        {
            EnsureAuthState();
            return Ok(EnumExtensions.GetValues<Gender>());
        }

        [HttpGet]
        [Route("user-titles")]
        public IActionResult UserTitle()
        {
            EnsureAuthState();
            return Ok(EnumExtensions.GetValues<UserTitle>().OrderBy(x => x.Name));
        }
    }
}
