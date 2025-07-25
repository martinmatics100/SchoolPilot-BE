

using SchoolPilot.Infrastructure.Validators;
using System.Net;

namespace SchoolPilot.Infrastructure.BasicResults
{
    public abstract class BaseValidationResult : IValidationResult
    {
        public HttpStatusCode Status { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<IValidationError> Errors { get; set; }
        public IEnumerable<IValidationError> Warnings { get; set; }
        protected BaseValidationResult(HttpStatusCode status)
        {
            Status = status;
        }

        public BaseValidationResult()
        {
            Status = HttpStatusCode.OK;
        }

        public BaseValidationResult(string errorMessage) : this(HttpStatusCode.BadRequest)
        {
            ErrorMessage = "Validation Failed";
            Errors = new List<IValidationError>
            {
                new ValidationError("", errorMessage)
            };
        }

        public BaseValidationResult(string errorMessage, string propertyName) : this(HttpStatusCode.BadRequest)
        {
            ErrorMessage = "Validation Failed";
            Errors = new List<IValidationError>
            {
                new ValidationError(propertyName, errorMessage)
            };
        }

        public BaseValidationResult(string errorMessage, string propertyName, string propertyErrorMessage) : this(HttpStatusCode.BadRequest)
        {
            ErrorMessage = errorMessage;
            Errors = new List<IValidationError>
            {
                new ValidationError(propertyName, propertyErrorMessage)
            };
        }
    }
}
