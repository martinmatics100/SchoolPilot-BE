

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Common.Enums;
using SchoolPilot.Api.Filters;
using SchoolPilot.Infrastructure.Commands.School;
using Microsoft.AspNetCore.Http;
using SchoolPilot.Data.Entities;
using SchoolPilot.Infrastructure.Queries.Accounts;
using SchoolPilot.Infrastructure.Queries.Schools;

namespace SchoolPilot.Api.Controllers.StandAlone
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/school-setup")]
    [ApiController]
    public class SchoolSetupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchoolSetupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("")]
        [ValidateModel]
        public async Task<IActionResult> CreateSchoolAccountAsync([FromBody] CreateSchoolAccount.Request query)
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
