

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Schools
{
    public class SchoolSetting : ModeledEntity, IEntity, IAuditable, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [Required]
        public Guid SchoolId { get; set; }

        public TasksToBypass TasksToBypass { get; set; }

        public bool AllowNewStudentRegistrations { get; set; }

        [MaxLength(40)]
        public string? SubmitterId { get; set; }

        [MaxLength(100)]
        public string? SubmitterName { get; set; }

        public virtual SchoolSettingsPhoneNumbers SubmitterPhoneNumber { get; set; }

        [ForeignKey("SubmitterPhoneNumber")]
        public Guid? SubmitterPhoneNumber_Id { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchoolSetting>()
                .AddIndex(builder => builder
                    .HasIndex(providerSettings => providerSettings.SchoolId));

        }

    }

    public class SchoolSettingsPhoneNumbers : PhoneNumber
    {
        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
