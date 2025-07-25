

using System.Net;

namespace SchoolPilot.Infrastructure.BasicResults
{
    public class BasicValidationResult : BaseValidationResult
    {

        public BasicValidationResult(string errorMessage, string propertyName) : base(errorMessage, propertyName) { }

        public BasicValidationResult(HttpStatusCode status) : base(status) { }
    }
}
