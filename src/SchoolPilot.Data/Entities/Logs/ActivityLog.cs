

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Data.Attributes;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Entities.Logs
{
    public class ActivityLog : ModeledEntity, IEntity, IAuditable, IAccountScope
    {
        private const string IdxDomainTypeEntityType = "IDX_DomainType_EntityType";
        private const string IdxAccountIdUserId = "IDX_AccountId_UserId";

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public Guid DomainId { get; set; }
        public ActivityDomainType DomainType { get; set; }
        public Guid EntityId { get; set; }
        public ActivityEntityType EntityType { get; set; }
        public int ActionId { get; set; }

        private string _summary;

        [MaxLength(255)]
        public string Summary
        {
            get
            {
                if(_summary == null)
                {
                    _summary = EntityType.GetDescription() + "-" + EntityType.GetAttribute<ActivityEntityType, ActivityLogActionListAttribute>().ActionEnum.GetEnumNames()[ActionId - 1].SplitOnCamelCase();
                }

                return _summary;
            }
            set => _summary = value;
        }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [MaxLength(254)]
        public string EntityName { get; set; }

        /// <summary>
        /// Json extra details if needed by the log
        /// </summary>
        public string? ExtraDetails { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>()
                .AddIndex(builder => builder
                    .HasIndex(log => log.EntityId)
                )
                .AddIndex(builder => builder
                    .HasIndex(log => log.CreatedOn)
                )
                .AddIndex(builder => builder
                    .HasIndex(log => new
                    {
                        log.AccountId,
                        log.UserId
                    })
                    .HasDatabaseName(IdxAccountIdUserId)
                )
                .AddIndex(builder => builder
                    .HasIndex(log => new
                    {
                        log.DomainType,
                        log.EntityType
                    })
                    .HasDatabaseName(IdxDomainTypeEntityType)
                )
                .Property(log => log.Summary)
                .HasField(nameof(_summary));
        }

    }

    public enum ActivityDomainType
    {
        School = 1,        // School-wide actions
        Student = 2,       // Student-related actions
        Teacher = 3,       // Teacher-related actions
        Class = 4,         // Class/grade-level actions
        Course = 5,        // Individual course actions
        Assignment = 6,    // Homework/tests
        Attendance = 7,    // Attendance records
        Parent = 8,       // Parent/guardian actions
        Payment = 9,       // Tuition/fee payments
        User = 10,
    }

    public enum ActivityEntityType
    {
        [ActivityLogActionList(typeof(ActivityLogStudentAction))]
        [ActivityDomainType(Student, ActivityDomainType.Student)]
        Student = 1,

        [ActivityLogActionList(typeof(ActivityLogTeacherAction))]
        [ActivityDomainType(Teacher, ActivityDomainType.Teacher)]
        Teacher = 2,

        [ActivityLogActionList(typeof(ActivityLogCourseAction))]
        [ActivityDomainType(Course, ActivityDomainType.Course)]
        Course = 3,

        [ActivityLogActionList(typeof(ActivityLogAssignmentAction))]
        [ActivityDomainType(Assignment, ActivityDomainType.Assignment)]
        Assignment = 4,

        [ActivityLogActionList(typeof(ActivityLogGradeAction))]
        [Description("Grade Entry")]
        [ActivityDomainType(Grade, ActivityDomainType.Student)]
        Grade = 5,

        [ActivityLogActionList(typeof(ActivityLogUserAction))]
        [ActivityDomainType(User, ActivityDomainType.User)]
        User = 2,
    }

    public enum ActivityLogStudentAction
    {
        CreateStudent = 1,
        UpdateStudent = 2,
        EnrollStudent = 3,
        TransferStudent = 4,
        GraduateStudent = 5,
        SuspendStudent = 6,
        DeleteStudent = 7,
    }

    public enum ActivityLogTeacherAction
    {
        CreateTeacher = 1,
        UpdateTeacher,
        DeleteTeacher,
        SetTeacherToActive,
    }

    public enum ActivityLogAssignmentAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Submit = 4,
        Grade = 5, 
        Return = 6    
    }

    public enum ActivityLogCourseAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Submit = 4, 
        Grade = 5, 
        Return = 6 
    }

    public enum ActivityLogGradeAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Submit = 4,
        Grade = 5,
        Return = 6  
    }

    public enum ActivityLogUserAction
    {
        Create = 1,
        Update,
        Login,
        UpdatePermissions,
        RevokePending,
        CreateInterim,
        AddToLocation,
        UpdateQaSettings,
        UserRoleApplied,
        UpdateCoSignatureSettings,
        RestorePendingUser
    }
}
