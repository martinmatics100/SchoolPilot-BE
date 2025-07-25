

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Staffs
{
    public class Staff : ModeledEntity, IEntity, IAccountScope, IAuditable, IArchivable
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }

        public Guid SchoolAccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public SchoolDepartment Department { get; set; }

        public StaffDesignation Designation { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set;}

        public bool IsDeprecated { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff>()
                .AddIndex(builder => builder
                    .HasIndex(user => user.SchoolAccountId)
                    .IsUnique())
                .AddIndex(builder => builder
                    .HasIndex(user => user.Email));
        }
    }
}
