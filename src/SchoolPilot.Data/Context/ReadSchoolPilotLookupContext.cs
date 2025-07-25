using Microsoft.EntityFrameworkCore;

namespace SchoolPilot.Data.Context
{
    public class ReadSchoolPilotLookupContext : SchoolPilotLookupContext
    {
        public ReadSchoolPilotLookupContext() : base("ReadSchoolPilotLookupContext") { }

        public ReadSchoolPilotLookupContext(DbContextOptions<SchoolPilotLookupContext> contextOptions) : base(contextOptions) { }

        [Obsolete("Cannot use the save changes on the read context.", true)]
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        [Obsolete("Cannot use the save changes on the read context.", true)]
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new Exception($"Cannot save using the {nameof(ReadSchoolPilotLookupContext)}. No writes can be used with this context.");
        }
    }
}
