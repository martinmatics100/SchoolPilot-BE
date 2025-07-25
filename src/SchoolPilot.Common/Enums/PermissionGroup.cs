

using SchoolPilot.Common.Attributes;
using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum PermissionGroup
    {
        // 1. CORE ADMINISTRATION (System-Wide Management)

        [PermissionsInGroup(ParentPermission.Users, ParentPermission.Teachers ,ParentPermission.Settings, ParentPermission.Students)]
        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [Description("Administration")]
        Administration = 1,

        // 2. ACADEMIC MANAGEMENT (Teaching/Learning)

        [PermissionsInGroup(ParentPermission.Courses, ParentPermission.Exams, ParentPermission.Assignments, 
            ParentPermission.Timetable, ParentPermission.Classes)]
        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete, PermissionActions.Export)]
        [Description("Academics")]
        Academics = 2,

        [PermissionsInGroup(ParentPermission.Students, ParentPermission.Attendance)]
        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [Description("Student Management")]
        StudentManagement = 3,

        [PermissionsInGroup(ParentPermission.StudentsReport, ParentPermission.StaffReports, ParentPermission.FinanceReports, 
            ParentPermission.AcademicReports, ParentPermission.LibraryReports, ParentPermission.AdministrativeReports, ParentPermission.EventsAndActivitiesReports,
            ParentPermission.AdmissionReports)]
        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [Description("Reports")]
        Reports = 4,
    }
}
