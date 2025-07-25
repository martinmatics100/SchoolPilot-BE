

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities
{
    public class UserDefaultSchool : ModeledEntity, IEntity, IAuditable, IAccountScope
    {
        private const string IdxUserIdAccountId = "IDX_UserDefaultProviders_UserId_AccountId";

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }

        public Guid SchoolId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDefaultSchool>()
                .AddIndex(builder => builder.HasIndex(period => new
                {
                    period.UserId,
                    period.AccountId
                })
                    .HasDatabaseName(IdxUserIdAccountId));
        }
    }
}
