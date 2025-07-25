

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Entities.Users
{
    public class UserAffiliationHistory : ModeledEntity, IEntity, IAuditable, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid UserAffiliationId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(250)]
        public string Comments { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public UserStatus? Status { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAffiliationHistory>()
                .AddIndex(builder => builder
                    .HasIndex(affiliationHistory => affiliationHistory.UserAffiliationId)
                    .HasDatabaseName("IDX_UserAffiliationHistory_UserAffiliationId"))
                .AddIndex(builder => builder.HasIndex(affiliation => affiliation.AccountId));
        }
    }
}
