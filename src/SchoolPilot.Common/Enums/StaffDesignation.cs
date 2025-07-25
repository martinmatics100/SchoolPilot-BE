

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum StaffDesignation
    {
        [Description("Teacher")]
        SubjectTeacher = 1,
        [Description("Principal")]
        Principal = 2, 
        [Description("Accountant")]
        Accountant =3,
        [Description("Non-Academic Staff")]
        NonAcademicStaff = 4
    }
}
