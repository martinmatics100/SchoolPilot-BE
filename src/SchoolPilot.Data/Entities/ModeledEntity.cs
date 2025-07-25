

using Microsoft.EntityFrameworkCore;

namespace SchoolPilot.Data.Entities
{
    public abstract class ModeledEntity
    {
        internal abstract void OnModelCreating(ModelBuilder modelBuilder);
    }
}
