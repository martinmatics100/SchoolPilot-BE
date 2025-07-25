

using System.ComponentModel.DataAnnotations;
using SchoolPilot.Data.Entities;

namespace SchoolPilot.Data.Interfaces
{
    public interface IContact<T> where T : PhoneNumber
    {
        ICollection<T> PhoneNumbers { get; set; }

        [MaxLength(254)]
        string Email { get; set; }
    }
}
