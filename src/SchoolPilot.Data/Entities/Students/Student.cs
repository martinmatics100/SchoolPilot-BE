

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Entities.Schools;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Students
{
    public class Student : ModeledEntity, IEntity, IAccountScope, IAuditable, IArchivable
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid SchoolId { get; set; }
        public Guid SchoolLocationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }

        public string HashedPassword { get; set; }

        public Guid? LoginId { get; set; }

        [ForeignKey("StudentAddress")]
        public Guid? StudentAddress_Id { get; set; }

        public virtual Address StudentAddress { get; set; }
        public string ClassRoom { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeprecated { get; set; }

        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .AddIndex(builder => builder
                    .HasIndex(user => user.AccountId))
                .AddIndex(builder => builder
                    .HasIndex(user => user.SchoolId));
        }
    }

    [Flags]
    public enum StudentMiscFlag
    {
        None = 1,
        IsOnline,
        IsSuspended
    }
}
