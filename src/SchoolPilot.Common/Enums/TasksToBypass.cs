

namespace SchoolPilot.Common.Enums
{
    public enum TasksToBypass
    {
        None = 0,
        BypassAttendanceSubmission = 1 << 0,        // 1
        BypassLessonPlanSubmission = 1 << 1,        // 2
        BypassGradeSubmission = 1 << 2,             // 4
        BypassParentContact = 1 << 3,               // 8
        BypassFacultyMeetings = 1 << 4,             // 16
        BypassProfessionalDevelopment = 1 << 5,     // 32
        BypassHallwayMonitoring = 1 << 6,           // 64
        BypassLunchDuty = 1 << 7,                   // 128

        // Combined permissions examples:
        SeniorTeacherPermissions = BypassAttendanceSubmission |
                                 BypassHallwayMonitoring |
                                 BypassLunchDuty,

        SpecialistPermissions = BypassLessonPlanSubmission |
                              BypassParentContact
    }
}
