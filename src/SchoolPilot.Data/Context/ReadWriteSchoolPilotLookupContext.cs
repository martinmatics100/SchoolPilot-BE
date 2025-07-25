using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SchoolPilot.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPilot.Data.Context
{
    public class ReadWriteSchoolPilotLookupContext : SchoolPilotLookupContext
    {
        public ReadWriteSchoolPilotLookupContext() : base("ReadWriteSchoolPilotLookupContext") { }

        public ReadWriteSchoolPilotLookupContext(DbContextOptions<SchoolPilotLookupContext> contextOptions) : base(contextOptions) { }

        public override int SaveChanges()
        {
            PreSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            PreSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void PreSaveChanges()
        {
            foreach (var entry in GetOfType<object>())
            {
                SaveActionHelper.ApplySaveActions(entry.Entity, entry.State, entry.Entry);
            }
        }

        private IEnumerable<TypeToEntry<TType>> GetOfType<TType>()
        {
            return ChangeTracker.Entries()
                .Select(x => new TypeToEntry<TType>
                {
                    Entity = (TType)x.Entity,
                    State = x.State,
                    Entry = x
                });
        }

        private class TypeToEntry<TType>
        {
            public TType Entity { get; set; }
            public EntityState State { get; set; }
            public EntityEntry Entry { get; set; }
        }
    }
}
