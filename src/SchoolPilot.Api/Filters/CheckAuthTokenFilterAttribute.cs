

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using NLog;
//using StackExchange.Redis;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace SchoolPilot.Api.Filters
//{
//    public class CheckAuthTokenFilterAttribute : ActionFilterAttribute, IOrderedFilter
//    {
//        public int Order => 1;
//        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
//        private readonly IConfiguration _configuration;
//        private readonly string _responseMessage = "Auth token is currently being used.";
//        private const string LockedAuthTokenFieldName = "lockedAuthToken";

//        public CheckAuthTokenFilterAttribute(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            if (_configuration["App.Environment"] == "Local" || _configuration["App.Environment"] == "Development")
//            {
//                await next();
//                return;
//            }

//            if (context.HttpContext.User.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
//            {
//                context.Result = new ObjectResult(_responseMessage) { StatusCode = 423 };
//                return;
//            }

//            var subjectClaim = identity.FindFirst("sub");

//            if (subjectClaim == null)
//            {
//                if (identity.HasClaim("scope", "trusted"))
//                {
//                    await next();
//                    return;
//                }
//                context.Result = new ObjectResult(_responseMessage) { StatusCode = 423 };
//                return;
//            }

//            var authTimeClaim = identity.FindFirst("auth_time")?.Value;
//            if (string.IsNullOrEmpty(authTimeClaim))
//            {
//                context.Result = new ObjectResult(_responseMessage) { StatusCode = 423 };
//                return;
//            }

//            var redisDatabase = context.HttpContext.RequestServices.GetService<IConnectionMultiplexer>()?.GetDatabase();
//            if (redisDatabase == null)
//            {
//                Logger.Error("Redis connection failed");
//                await next();
//                return;
//            }

//            try
//            {
//                var authHashValue = GenerateSHA(authTimeClaim);
//                var hashKey = $"{LockedAuthTokenFieldName}:{subjectClaim.Value}:{authHashValue}";

//                if (await redisDatabase.KeyExistsAsync(hashKey))
//                {
//                    context.Result = new ObjectResult(_responseMessage) { StatusCode = 423 };
//                    return;
//                }
//            }
//            catch (RedisException exception)
//            {
//                Logger.Error(exception);
//            }

//            await next();
//        }

//        private static string GenerateSHA(string authTime)
//        {
//            using var sha1 = SHA1.Create();
//            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(authTime));
//            return BitConverter.ToString(hash).Replace("-", "");
//        }
//    }
//}
