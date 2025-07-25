

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Data.Entities.Schools
{
    public class SchoolSubject : ModeledEntity, IEntity, IAccountScope, IAuditable, IArchivable
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; }

        [MaxLength(20)]
        public string? SubjectCode { get; set; }

        public Guid? StaffId { get; set; }

        [MaxLength(20)]
        public List<SchoolLevel> Level { get; set; }

        [MaxLength(50)]
        public List<SubjectCategory> Category { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn {  get; set; }

        public bool IsDeprecated { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchoolSubject>()
                .AddIndex(builder => builder
                .HasIndex(inquiry => inquiry.AccountId)
                )
                .AddIndex(builder => builder
                .HasIndex(sub => sub.SchoolId));
        }
    }
}
