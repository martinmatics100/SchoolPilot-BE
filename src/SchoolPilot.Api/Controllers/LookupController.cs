

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPilot.Business.BasicResults;
using SchoolPilot.Common.Enums;
using SchoolPilot.Infrastructure.Helpers;
using SchoolPilot.Infrastructure.Queries.CustomEnums;
using StackExchange.Redis.Extensions.Core.Abstractions;
using SchoolPilot.Common.Helpers;
using SchoolPilot.Infrastructure.Queries.General;

namespace SchoolPilot.Api.Controllers
{
    [ApiVersion((int)ApiVersions.v1)]
    [Route("api/{version}/lookup")]
    [ApiController]
    public class LookupController : BaseApiController
    {
        private readonly IPermissionMapper _permissionMapper;
        private readonly IMediator _mediator;

        public LookupController(IPermissionMapper permissionMapper, IMediator mediator, IRedisClient redisCache) : base(mediator, redisCache)
        {
            _permissionMapper = permissionMapper;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("phone-number-type")]
        public async Task<Microsoft.AspNetCore.Mvc.IActionResult> PhoneNumberType()
        {
            var query = new GetPhoneNumberType.Query();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet]
        [Route("permissiondependencies")]
        public Microsoft.AspNetCore.Mvc.IActionResult PermissionDependencies()
        {
            return Ok(_permissionMapper.GetPermissionDependencies());
        }

        [HttpGet]
        [Route("permissiongroups")]
        public Microsoft.AspNetCore.Mvc.IActionResult PermissionGroups()
        {
            return Ok(_permissionMapper.GetPermissionGroups());
        }

        [HttpGet]
        [Route("countries")]
        public Microsoft.AspNetCore.Mvc.IActionResult Countries()
        {
            var result = Common.Helpers.Countries.Lookup()
                .Values
                .Select(s => new
                {
                    name = s.Name,
                    value = s.NumericCode,
                    twoLetterCode = s.TwoLetterCode
                });
            
            return Ok(result);
        }

        [HttpGet]
        [Route("country-phone-codes")]
        public Microsoft.AspNetCore.Mvc.IActionResult CountryPhoneCodes()
        {
            var result = Common.Helpers.Countries.LookupPhone()
                .Values
                .Select(s => new
                {
                    name = s.Name,
                    phoneCode = s.CountryCode,
                    value = s.NumericCode,
                    twoLetterCode = s.TwoLetterCode
                });

            return Ok(result);
        }

        [HttpGet]
        [Route("schoolpilot-countries")]
        public async Task<Microsoft.AspNetCore.Mvc.IActionResult> GetCountryAsync()
        {
            var query = new GetCountries.Query();

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpGet]
        [Route("schoolpilot-states")]
        public async Task<Microsoft.AspNetCore.Mvc.IActionResult> GetStatesAsync([FromQuery] Guid Id)
        {
            var query = new GetStates.Query
            {
                CountryId = Id
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
