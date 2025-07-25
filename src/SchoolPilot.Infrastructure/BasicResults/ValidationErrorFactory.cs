using SchoolPilot.Infrastructure.BasicResults;
using SchoolPilot.Infrastructure.Validators;

namespace SchoolPilot.Infrastructure.BasicResults
{
    public interface IValidationErrorFactory
    {
        IValidationError Create(string propertyName, string message);
    }

    public class ValidationErrorFactory : IValidationErrorFactory
    {
        public IValidationError Create(string propertyName, string message)
        {
            return new ValidationError(propertyName, message);
        }
    }
}

//USAGE BELOW

//public class SomeValidator
//{
//    private readonly IValidationErrorFactory _validationErrorFactory;

//    public SomeValidator(IValidationErrorFactory validationErrorFactory)
//    {
//        _validationErrorFactory = validationErrorFactory;
//    }

//    public IValidationError ValidateSomething(string input)
//    {
//        if (string.IsNullOrEmpty(input))
//        {
//            return _validationErrorFactory.Create("Input", "Input cannot be empty");
//        }
//        return null;
//    }
//}
