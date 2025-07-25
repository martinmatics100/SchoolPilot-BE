

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Logs
{
    public class ActivityLogDetails : ModeledEntity, IEntity
    {
        public Guid Id { get; set; }

        public Guid ActivityLogId { get; set; }

        public Guid AccountId { get; set; }

        public string PropertyDisplayName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public EntryType EntryType { get; set; }

        //Either the ActivityLogId or another ActivityLogDetailId if its a subsection
        public Guid? ParentId { get; set; }

        public SubEntityType? SubEntityType { get; set; }

        public Guid? SubEntityId { get; set; }

        public ActionType ActionType { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLogDetails>()
                .AddIndex(builder => builder
                    .HasIndex(log => log.ActivityLogId)
                )
                .AddIndex(builder => builder
                    .HasIndex(log => log.AccountId)
                );
        }

    }


    public enum SubEntityType
    {
        Address = 0,
        Contact = 1,
        Class = 2,
        Subject = 3,
        Teacher = 4,
        Student = 5,
        Parent = 6,
        Attendance = 7,
        Grade = 8,
        Timetable = 9,
        Exam = 10,
        Result = 11,
        Fee = 12,
        Payment = 13,
        Notice = 14,
        Event = 15,
        Assignment = 16,
        CourseMaterial = 17,
        SchoolBranch = 18,
        Role = 19,
        UserAccount = 20,
        SchoolSettings = 21,
        Term = 22,
        Session = 23
    }



    [Flags]
    public enum EntryType
    {
        Field = 1,
        SubEntity = 1 << 1,
        List = 1 << 2,
        FlaggedEnum = 1 << 3,
        Hidden = 1 << 4
    }

    public enum ActionType
    {
        Added = 1,
        Updated,
        Removed,
        Deactivated
    }

}
