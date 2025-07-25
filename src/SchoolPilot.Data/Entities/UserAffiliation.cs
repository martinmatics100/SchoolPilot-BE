

using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SchoolPilot.Common.Enums;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using JetBrains.Annotations;

namespace SchoolPilot.Data.Entities
{
    public class UserAffiliation : ModeledEntity, IEntity, IAuditable, IArchivable, IAccountScope
    {
        private const string UserIdAccountIndexName = "UIX_UserAffiliations_UserId_AccountId";

        public Guid Id { get; set; }

        public virtual User User { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public virtual Account Account { get; set; }

        [Required]
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }

        [MaxLength(254)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(1)]
        public string? MiddleInitial { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public UserStatus Status { get; set; }

        public UserTitle Title { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfHire { get; set; }

        [ForeignKey("HomePhone")]
        public Guid? HomePhone_Id { get; set; }

        public virtual UserAffiliationPhoneNumber HomePhone { get; set; }

        [ForeignKey("MobilePhone")]
        public Guid? MobilePhone_Id { get; set; }

        public virtual UserAffiliationPhoneNumber MobilePhone { get; set; }

        [ForeignKey("Address")]
        public Guid? Address_Id { get; set; }

        public virtual Address Address { get; set; }

        public int? EarliestLoginTimeInMinutes { get; set; }

        public int? AutomaticLogoutTimeInMinutes { get; set; }

        public bool AllowWeekendAccess { get; set; }

        public short TimeZoneOffset { get; set; }


        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public UserRole Role { get; set; }

        public bool IsDeprecated { get; set; }

        public bool AllowEmergencyAccessRequest { get; set; }

        public bool AllowReadOnlyAccess { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAffiliation>()
                .AddIndex(builder => builder
                    .HasIndex(user => user.Title)
                )
                .AddIndex(builder => builder
                    .HasIndex(user => user.Status)
                )
                .AddIndex(builder => builder
                    .HasIndex(user => new
                    {
                        user.UserId,
                        user.AccountId
                    })
                    .HasDatabaseName(UserIdAccountIndexName)
                    .IsUnique()
                )
                .AddIndex(builder => builder.HasIndex(user => user.AccountId))
                .AddIndex(builder => builder.HasIndex(user => user.Email));
        }
    }

    [UsedImplicitly]
    [Table("UserAffiliationPhoneNumber")]
    public class UserAffiliationPhoneNumber : PhoneNumber
    {
        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
