

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities
{
    public class User : ModeledEntity, IEntity, IAuditable, IArchivable
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeprecated { get; set; }

        //public Guid SchoolAccountId { get; set; }

        /// <summary>
        /// The time of the last modification based upon the token creation datetime.
        /// If this field is null, the user of this subject id has never made a request against the API.
        /// </summary>
        public DateTime? LastValidated { get; set; }

        public Guid? LoginId { get; set; }

        public UserRole Role { get; set; }

        [MaxLength(50)]
        public string InviteId { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .AddIndex(builder => builder
                    .HasIndex(user => user.LoginId)
                    .IsUnique())
                .AddIndex(builder => builder
                    .HasIndex(user => user.Email));
        }

    }
}
