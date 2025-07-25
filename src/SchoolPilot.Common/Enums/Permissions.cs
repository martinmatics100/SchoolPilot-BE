

using SchoolPilot.Common.Attributes;
using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum ParentPermission
    {
        // Administration Permissions
        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Users")]
        Users = 1,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Teachers & Staff")]
        Teachers = 2,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Students")]
        Students = 3,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Courses & Subjects")]
        Courses = 4,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Classes & Sections")]
        Classes = 5,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [Description("Attendance")]
        Attendance = 6,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Exams & Grades")]
        Exams = 7,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Timetable")]
        Timetable = 8,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [PermissionDependency(PermissionActions.Delete, PermissionActions.View)]
        [Description("Assignments")]
        Assignments = 9,

        [AvailableActions(PermissionActions.View)]  // Parents can only view (no add/edit/delete)
        [Description("Parent Portal")]
        ParentPortal = 10,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Add, PermissionActions.View, PermissionActions.Edit)]
        [PermissionDependency(PermissionActions.Edit, PermissionActions.View, PermissionActions.Add)]
        [Description("School Settings")]
        Settings = 11,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Export)]
        [Description("Fee Management")]
        Fees = 12,

        [AvailableActions(PermissionActions.View, PermissionActions.Add, PermissionActions.Edit, PermissionActions.Delete)]
        [Description("Library")]
        Library = 13,

        // Report Permissions
        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Students Report")]
        StudentsReport = 14,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Staff Reports")]
        StaffReports = 15,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Academic Reports")]
        AcademicReports = 16,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Finance Reports")]
        FinanceReports = 17,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Library Reports")]
        LibraryReports = 18,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Admission Reports")]
        AdmissionReports = 19,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Administrative Reports")]
        AdministrativeReports = 21,

        [AvailableActions(PermissionActions.View, PermissionActions.Export)]
        [PermissionDependency(PermissionActions.Export, PermissionActions.View)]
        [Description("Events & Activities Reports")]
        EventsAndActivitiesReports = 22,
    }

    public enum PermissionActions
    {
        // Common Permission Actions
        [Description("View")]
        View = 1,
        [Description("Add")]
        Add = 2,
        [Description("Edit")]
        Edit = 3,
        [Description("Delete")]
        Delete = 4,
        [Description("Export")]
        Export = 8,

        // Clinical Only Permission Actions
        //[Description("Reassign")]
        //Reassign = 5,

        // Billing Only Permission Actions
        //[Description("Export ANSI")]
        //ExportAnsi = 6,
        //[Description("E-Submission")]
        //ESubmission = 7,
    }

    public enum ReadonlyPermissionActions
    {
        // Common Permission Actions
        [Description("View")]
        View = 1,
        [Description("Export")]
        Export = 8,

        //// Billing Only Permission Actions
        //[Description("Export ANSI")]
        //ExportAnsi = 6,
    }

}
