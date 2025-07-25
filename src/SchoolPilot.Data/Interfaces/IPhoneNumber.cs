

using SchoolPilot.Common.Enums;

namespace SchoolPilot.Data.Interfaces
{
    public interface IPhoneNumber
    {
        string Number { get; set; }

        string Country { get; set; }

        PhoneType Type { get; set; }

        string Format();
    }
}
