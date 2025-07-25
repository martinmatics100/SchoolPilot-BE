

using SchoolPilot.Common.Attributes;

namespace SchoolPilot.Data.Interfaces
{
    public interface IArchivable
    {
        [NotAutoMapped]
        bool IsDeprecated { get; set; }
    }
}
