
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolPilot.Infrastructure.BasicResults;
using SchoolPilot.Infrastructure.Validators;
using System.Net;
using Newtonsoft.Json;

namespace SchoolPilot.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute, IOrderedFilter
    {
        public int Order => 6;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ValidationResultModel(context.ModelState));
            }

            base.OnActionExecuting(context);
        }

        public class ValidationResultModel : IValidationResult
        {
            [JsonIgnore]
            public HttpStatusCode Status { get; set; }

            public string ErrorMessage { get; set; }

            public IEnumerable<IValidationError> Errors { get; set; }

            public ValidationResultModel()
            {
                ErrorMessage = "Validation Failed";
                Status = HttpStatusCode.BadRequest;
            }

            public ValidationResultModel(ModelStateDictionary modelState) : this()
            {
                Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.GroupBy(g => g.ErrorMessage).Select(x => new ValidationError(key, x.First().ErrorMessage)))
                    .ToList();
            }
        }
    }
}
