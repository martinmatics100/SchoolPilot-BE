

using System.Net;
using Newtonsoft.Json;
using SchoolPilot.Infrastructure.Validators;

namespace SchoolPilot.Infrastructure.BasicResults
{
    public interface IValidationResult
    {

        [JsonIgnore]
        HttpStatusCode Status { get; set; }

        [JsonIgnore]
        string ErrorMessage { get; set; }

        IEnumerable<IValidationError> Errors { get; set; }
    }
}
