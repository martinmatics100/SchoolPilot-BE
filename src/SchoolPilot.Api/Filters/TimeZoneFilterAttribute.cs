

using Microsoft.AspNetCore.Mvc.Filters;
using SchoolPilot.Api.Controllers;
using SchoolPilot.Api.Interfaces;

namespace SchoolPilot.Api.Filters
{
    public class TimeZoneFilterAttribute : ActionFilterAttribute, IOrderedFilter
    {
        public int Order => 2;
        private const string XTimeZone = "X-Time-Zone";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is IBaseApiController controller)
            {
                // Restore all previous auth data
                if (context.HttpContext.Items.TryGetValue("AuthData", out var authDataObj) &&
                    authDataObj is AccountScopeFilterAttribute.AuthData authData)
                {
                    controller.UserId = authData.UserId;
                    controller.CurrentUser = authData.CurrentUser;
                    controller.AccountId = authData.AccountId;
                }

                // Process timezone
                if (context.HttpContext.Request.Headers.TryGetValue(XTimeZone, out var timeZoneHeader))
                {
                    controller.TimeZoneString = timeZoneHeader.FirstOrDefault();
                }
            }

            await next();
        }
    }
}
