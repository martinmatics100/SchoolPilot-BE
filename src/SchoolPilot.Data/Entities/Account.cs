
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Entities
{
    // Account entity for all Schools on-boarded on SCHOOL-PILOT
    public class Account : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [StringLength(256)]
        [Required]
        public string Name { get; set; }

        public UserTitleExtension OwnerTitle { get; set; }

        public string? SchooOwnerFirstName { get; set; }

        public string? SchoolOwnerLastName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public AccountMiscFlags Flags { get; set; }

        /// <summary>
        /// Future date to trigger the disabling of an account.
        /// </summary>
        //public DateTime? SuspendDate { get; set; }

        /// <summary>
        /// UTC DateTime for when an account is disabled.
        /// </summary>
        //public DateTime? DisabledOn { get; set; }
    }

    [Flags]
    public enum AccountMiscFlags
    {
        None = 0,
        IsTestingAccount = 1,
        IsDisabled = 1 << 1,
    }
}
