

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Infrastructure.Commands.School;
using SchoolPilot.Infrastructure.Commands.Users;

namespace SchoolPilot.Api.Controllers.StandAlone
{
    [ApiVersion((int)ApiVersions.v2)]
    [Route("api/{version}/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUser.Request query)
        {
            var result = await _mediator.Send(query);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
