

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Entities.Users
{
    public class Permission : ModeledEntity, IEntity, IAccountScope
    {
        public Guid Id { get; set; }

        public Guid PrincipleId { get; set; }

        public Guid AccountId { get; set; }

        [MaxLength(36)]
        public string Resource { get; set; }

        public int ResourceType { get; set; }

        public int Action { get; set; }

        public PermissionValue Value { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .AddIndex(builder => builder
                    .HasIndex(permission => permission.AccountId)
                )
                .AddIndex(builder => builder
                    .HasIndex(permission => new
                    {
                        permission.PrincipleId,
                        permission.Resource,
                        permission.ResourceType,
                        permission.Action
                    })
                    .HasDatabaseName("IDX_PermissionPrinciple")
                    .IsUnique()
                );
        }
    }

    public enum PermissionValue
    {
        Granted = 0,
        Denied = 1
    }

    public enum ResourceType
    {
        Categorized = 1,
        Patient = 2,
        Report = 3
    }
}
