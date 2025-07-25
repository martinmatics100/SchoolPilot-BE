

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Schools
{

    // In Place of AgencyLocation Table
    public class SchoolLocation : ModeledEntity, IEntity, IAccountScope, IAuditable, IArchivable
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }

        public Guid SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public TimeZones TimeZone { get; set; }

        public virtual Address Address { get; set; }

        public virtual NewPhoneNumber PrimaryPhone { get; set; }

        public virtual BaseSchool School { get; set; }

        [ForeignKey("Address")]
        public Guid? Address_Id { get; set; }

        [ForeignKey("PrimaryPhone")]
        public Guid? PrimaryPhone_Id { get; set; }

        public bool IsMainLocation { get; set; }

        public bool IsDeprecated { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsReadOnly { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchoolLocation>()
                .AddIndex(builder => builder
                    .HasIndex(user => user.AccountId))
                .AddIndex(builder => builder
                    .HasIndex(user => new { user.SchoolId, user.Name}).IsUnique());

            modelBuilder.Entity<SchoolLocation>()
        .HasOne(sl => sl.PrimaryPhone)
        .WithMany() // Assuming NewPhoneNumber doesn't have a navigation property back to SchoolLocation
        .HasForeignKey(sl => sl.PrimaryPhone_Id);

        }
    }
}
