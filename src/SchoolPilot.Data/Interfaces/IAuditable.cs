

using SchoolPilot.Common.Attributes;
using SchoolPilot.Data.Attributes;

namespace SchoolPilot.Data.Interfaces
{
    public interface IAuditable
    {
        [NotAutoMapped]
        [AuditableCreationDate]
        DateTime? CreatedOn { get; set; }

        [NotAutoMapped]
        [AuditableModificationDate]
        DateTime? ModifiedOn { get; set; }
    }
}
