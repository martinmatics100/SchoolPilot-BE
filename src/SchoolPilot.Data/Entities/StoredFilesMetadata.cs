

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities
{
    public class StoredFilesMetadata : ModeledEntity, IEntity , IAccountScope, IArchivable, IAuditable
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; }
        public string PublicUrl { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set;}
        public bool IsDeprecated { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoredFilesMetadata>()
                .AddIndex(builder => builder
                    .HasIndex(file => file.AccountId)
                    .IsUnique())
                .AddIndex(builder => builder
                    .HasIndex(file => file.FileName));
        }
    }
}
