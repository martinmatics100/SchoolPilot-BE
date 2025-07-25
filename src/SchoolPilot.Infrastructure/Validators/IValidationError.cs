

using Newtonsoft.Json;

namespace SchoolPilot.Infrastructure.Validators
{
    public interface IValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string PropertyName { get; }

        string Message { get; }
    }
}
