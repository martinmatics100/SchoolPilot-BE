

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using SchoolPilot.Common.Extensions;
using NLog;
using SchoolPilot.Api.Controllers;
using SchoolPilot.Api.Models;
using System.Net;
using System.Security.Claims;
using SchoolPilot.Infrastructure.Queries.Accounts;

namespace SchoolPilot.Api.Filters
{
    public class UserAuthorizeFilterAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter, IOrderedFilter
    {
        public int Order => 0;

        private static readonly Logger Logegr = LogManager.GetCurrentClassLogger();
        private HashSet<string> _authorizedClients;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public UserAuthorizeFilterAttribute(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (_authorizedClients == null)
            {
                _authorizedClients = _configuration["Oidc:AuthorizedClients"]?.Split(';').ToHashSet() ?? new HashSet<string>();
            }

            var controllerType = context.ActionDescriptor.GetType().GetProperty("ControllerTypeInfo")?.GetValue(context.ActionDescriptor) as Type;

            var controller = context.HttpContext.RequestServices.GetService(controllerType) as IBaseApiController;

            if (controller != null)
            {
                if (controller.UserId.HasValue)
                {
                    return;
                }

                var authenticateStatus = await HandleOidcVerification(context, controller);
                var isSuccessful = authenticateStatus.ContainsFlag(AuthenticateStatus.Success);

                if (!isSuccessful)
                {
                    LogFailure(context, authenticateStatus);
                }

                if (isSuccessful || await HandleOtherVerification(context, controller))
                {
                    context.HttpContext.Items["AuthData"] = new AccountScopeFilterAttribute.AuthData
                    {
                        UserId = controller.UserId,
                        AccountId = controller.AccountId,
                        CurrentUser = controller.CurrentUser
                    };

                    return;
                }
            }

            context.Result = new ObjectResult("Invalid user credential or client is not authorized to use this endpoint. If this endpoint requires authentication then consider checking if token within the Authorization header is valid and has not expired.")
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }


        private static void LogFailure(AuthorizationFilterContext context, AuthenticateStatus authenticateStatus)
        {
            var message = "";
            if (authenticateStatus.ContainsFlag(AuthenticateStatus.SpecialUser))
            {
                message = "Unauthorized Special User: ";
            }
            else if (authenticateStatus.ContainsFlag(AuthenticateStatus.UserNotFound) &&
                     authenticateStatus.ContainsFlag(AuthenticateStatus.HospiceToken))
            {
                message = "Authorization Failed User Not Found: ";
            }
            else if (authenticateStatus.ContainsFlag(AuthenticateStatus.SubjectIdNonGuid) ||
                     authenticateStatus.ContainsFlag(AuthenticateStatus.SubjectIdMissing))
            {
                message = "Authorization Failed Subject Id Issue: ";
            }
            else if (authenticateStatus == AuthenticateStatus.InvalidToken)
            {
                message = "Invalid Authorization Token: ";
            }

            if (!string.IsNullOrEmpty(message))
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                LogManager.GetCurrentClassLogger().Debug("{Message}{AuthHeader}", message, authHeader?.Split(' ').LastOrDefault());
            }
        }

        private async Task<bool> HandleOtherVerification(AuthorizationFilterContext context, IBaseApiController controller)
        {
            var environment = _configuration["App:Environment"];
            // Only dev and local environments should allow the use of the login id as a valid authorization.
            if (environment != "Development" && environment != "Local")
            {
                return false;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (controller != null && authHeader?.StartsWith("loginid ") == true &&
                Guid.TryParse(authHeader.Substring(8), out var loginId))
            {
                var result = await _mediator.Send(new GetUserByLoginId.Query { LoginId = loginId });
                if (result == null || result.Id == Guid.Empty)
                {
                    return false;
                }

                controller.UserId = result.Id;
                controller.AccountId = result.AccountId;
                return true;
            }
            return false;
        }


        private async Task<AuthenticateStatus> HandleOidcVerification(AuthorizationFilterContext context, IBaseApiController controller)
        {
            var status = AuthenticateStatus.None;
            var identity = context.HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return AuthenticateStatus.InvalidToken;
            }

            var clientId = identity.Claims.FirstOrDefault(x => x.Type == "client_id");
            var clientIdValue = clientId?.Value;
            if (clientIdValue == _configuration["Oidc:ClientId"])
            {
                status = AuthenticateStatus.HospiceToken;
            }

            var subjectClaim = identity.Claims.FirstOrDefault(x => x.Type == "subs");
            if (subjectClaim == null)
            {
                // If there is no subject id then we might be dealing with a client credential flow auth token
                if (clientIdValue != null && _authorizedClients.Contains(clientIdValue) &&
                    identity.Claims.Any(x => x.Type == "scope" && x.Value == "trusted"))
                {
                    return AuthenticateStatus.OtherApplication | AuthenticateStatus.Success;
                }

                return AuthenticateStatus.SubjectIdMissing;
            }

            // subject id is actually the login id of the user.
            if (!Guid.TryParse(subjectClaim.Value, out var subjectId))
            {
                return AuthenticateStatus.SubjectIdNonGuid;
            }

            var result = await _mediator.Send(new GetUserByLoginId.Query { LoginId = subjectId });
            if (result == null || result.Id == Guid.Empty)
            {
                return status | AuthenticateStatus.UserNotFound;
            }

            controller.CurrentUser = new CurrentUser
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email
            };
            controller.UserId = result.Id;

            return status | AuthenticateStatus.Success;
        }
        [Flags]
        private enum AuthenticateStatus
        {
            None = 0,
            Success = 1,
            InvalidToken = 2,
            SubjectIdMissing = 4,
            SubjectIdNonGuid = 8,
            UserNotFound = 16,
            HospiceToken = 32,
            SpecialUser = 64,
            OtherApplication = 128
        }
    }
}
