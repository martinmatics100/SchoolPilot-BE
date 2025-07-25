

using Microsoft.EntityFrameworkCore;
using System.Text;

namespace SchoolPilot.Data.Extensions
{
    public static class ExceptionExtensions
    {
        public static string Format(this DbEntityValidationException ex)
        {
            return string.Join(";", ex.ValidationErrors.SelectMany(sm => sm.EntityValidationResults.Select(s =>
            {
                var memberNames = string.Join(",", s.MemberNames);
                return memberNames + ": " + s.ErrorMessage;
            })));
        }

        public static string Format(this DbUpdateException ex)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

            try
            {
                foreach (var result in ex.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem with the state of {1}. ", result.Entity.GetType().Name, result.State);
                }

                IterateExceptionMessages(builder, ex);
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e.ToString());
            }
            return builder.ToString();
        }

        private static void IterateExceptionMessages(StringBuilder builder, Exception e)
        {
            builder.AppendLine(e.GetType().FullName + ": " + e.Message);
            if (e.InnerException != null)
            {
                IterateExceptionMessages(builder, e.InnerException);
            }
        }

        public static bool IsUniqueConstraintException(this DbUpdateException exception)
        {
            return exception.InnerException?.Message?.Contains("Unique") == true;
        }
    }
}
