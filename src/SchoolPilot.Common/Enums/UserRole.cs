

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum UserRole
    {
        [Description("Administrator")]
        Admin = 1,
        [Description("Teacher")]
        Teacher,
        [Description("Student")]
        Student,
        [Description("Parent")]
        Parent,
    }
}
