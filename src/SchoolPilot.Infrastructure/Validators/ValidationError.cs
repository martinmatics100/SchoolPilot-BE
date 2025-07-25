

namespace SchoolPilot.Infrastructure.Validators
{
    public class ValidationError : IValidationError
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }

        // Parameterless constructor for DI
        public ValidationError() { }

        public ValidationError(string field, string message)
        {
            PropertyName = field != string.Empty ? field : null;
            Message = message;
        }
    }
}
