

using SchoolPilot.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Interfaces
{
    public interface IEntity
    {
        [Key]
        [GenerateSequentialId]
        Guid Id { get; set; }
    }
}
