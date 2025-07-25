

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Users
{
    public class Principle : ModeledEntity, IEntity
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public PermissionAccessType PermissionType { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Principle>()
                .AddIndex(builder => builder
                    .HasIndex(principle => principle.AccountId)
                );
        }
    }
}
