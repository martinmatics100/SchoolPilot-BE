
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Users
{
    public class UserPrinciple : ModeledEntity, IEntity, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [ForeignKey("UserAffiliation")]
        public Guid UserAffiliationId { get; set; }

        [ForeignKey("Principle")]
        public Guid PrincipleId { get; set; }

        public virtual UserAffiliation UserAffiliation { get; set; }

        public virtual Principle Principle { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPrinciple>()
                .AddIndex(builder => builder
                    .HasIndex(userPrinciple => userPrinciple.AccountId));
        }
    }
}
