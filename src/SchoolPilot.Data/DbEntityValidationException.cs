

using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data
{
    public class DbEntityValidationException : Exception
    {
        public readonly List<DbEntityValidationResult> ValidationErrors;

        public DbEntityValidationException(List<DbEntityValidationResult> validationResults)
        {
            ValidationErrors = validationResults;
        }
    }

    public class DbEntityValidationResult
    {
        public object Entity { get; }
        public ICollection<ValidationResult> EntityValidationResults { get; }

        public DbEntityValidationResult(object entity, ICollection<ValidationResult> entityValidationResult)
        {
            Entity = entity;
            EntityValidationResults = entityValidationResult;
        }
    }
}
