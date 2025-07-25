

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities
{
    public class UserLocation : ModeledEntity, IEntity, IAuditable, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Column("AccountId")]
        public Guid AccountId { get; set; }

        public Guid AgencyLocationId { get; set; }

        public bool IsPrimaryLocation { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLocation>()
                .AddIndex(builder => builder
                    .HasIndex(location => location.UserId)
                )
                .AddIndex(builder => builder
                    .HasIndex(location => location.AccountId).HasDatabaseName("Idx_UserLocation_AccountId")
                );
        }
    }
}
