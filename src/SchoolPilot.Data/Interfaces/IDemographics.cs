

using SchoolPilot.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Interfaces
{
    public interface IDemographics
    {
        [MaxLength(100)]
        string FirstName { get; set; }

        [MaxLength(100)]
        string LastName { get; set; }

        [MaxLength(1)]
        string MiddleInitial { get; set; }

        Gender Gender { get; set; }

        DateTime? DateOfBirth { get; set; }
    }
}
