
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolPilot.Api.Controllers;
using SchoolPilot.Api.Models;

namespace SchoolPilot.Api.Filters
{
    public class AccountScopeFilterAttribute : ActionFilterAttribute, IOrderedFilter
    {
        public int Order => 1;
        private const string XAccountId = "X-Account-Id";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is IBaseApiController controller)
            {
                // Preserve existing auth data from UserAuthorizeFilter
                if (context.HttpContext.Items.TryGetValue("AuthData", out var authDataObj) &&
                    authDataObj is AuthData authData)
                {
                    controller.UserId = authData.UserId;
                    controller.CurrentUser = authData.CurrentUser;
                }

                // Process account scope
                if (context.HttpContext.Request.Headers.TryGetValue(XAccountId, out var accountIdHeader) &&
                    Guid.TryParse(accountIdHeader.FirstOrDefault(), out var accountId))
                {
                    controller.AccountId = accountId;
                }

                // Update auth data for downstream filters
                context.HttpContext.Items["AuthData"] = new AuthData
                {
                    UserId = controller.UserId,
                    CurrentUser = controller.CurrentUser,
                    AccountId = controller.AccountId
                };
            }

            await next();
        }

        public class AuthData
        {
            public Guid? UserId { get; set; }
            public CurrentUser CurrentUser { get; set; }
            public Guid? AccountId { get; set; }
        }
    }
}
