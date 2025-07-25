

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Users
{
    public class Role : ModeledEntity, IEntity
    {
        public Guid Id { get; set; }
        public UserRole Name { get; set; }

        public string NormalizedName { get; set; } = string.Empty;

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .AddIndex(builder => builder
                    .HasIndex(role => role.Name));
        }
    }
}
