

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Payroll
{
    public class SchoolPayrollGroup : ModeledEntity, IEntity, IAccountScope, IAuditable
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid SchoolId { get; set; }

        public string Name { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchoolPayrollGroup>()
                .AddIndex(builder => builder.HasIndex(period => period.AccountId))
                .AddIndex(builder => builder.HasIndex(period => period.SchoolId));
        }
    }
}
