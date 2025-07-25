

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Schools
{

    // Replacing Providers
    public class BaseSchool : ModeledEntity, IEntity, IAuditable, IArchivable, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [StringLength(256)]
        [Required]
        public string SchoolName { get; set; }

        public SchoolTerms CurrentTerm { get; set; }

        public SchoolCategory SchoolCategory { get; set; }

        public SchoolType SchoolType { get; set; }

        public SchoolStatus SchoolStatus { get; set; }

        [Required]
        [MaxLength(300)]
        public string ContactPersonEmail { get; set; }

        public Guid ContactPersonPhone_Id { get; set; }

        public Guid? SchoolPrimaryAdress_Id { get; set; }

        [ForeignKey(nameof(ContactPersonPhone_Id))]
        public virtual SchoolPhoneNumber ContactPersonPhone { get; set; }

        [ForeignKey(nameof(SchoolPrimaryAdress_Id))]
        public virtual Address? SchoolPrimaryAdress { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Future date to trigger the disabling of an account.
        /// </summary>
        public DateTime? SuspendDate { get; set; }

        /// <summary>
        /// UTC DateTime for when an account is disabled.
        /// </summary>
        public DateTime? DisabledOn { get; set; }

        public bool IsDeprecated { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseSchool>()
                .AddIndex(builder => builder
                .HasIndex(inquiry => inquiry.AccountId));
        }
    }
}
