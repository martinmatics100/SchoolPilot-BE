using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Entities.lookup;
using System.Diagnostics;
using System.Reflection;

namespace SchoolPilot.Data.Context
{
    public abstract class SchoolPilotLookupContext : DbContext
    {
        public string? ConnectionStringName { get; }

        protected SchoolPilotLookupContext() { }

        protected SchoolPilotLookupContext(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        protected SchoolPilotLookupContext(DbContextOptions<SchoolPilotLookupContext> contextOptions) : base(contextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model
                .GetEntityTypes()
                .Where(w => w.ClrType.IsSubclassOf(typeof(ModeledEntity)));

            foreach (var entity in entities)
            {
                Console.WriteLine(entity);
                var entityModelDefiner = entity.ClrType.GetMethod(nameof(ModeledEntity.OnModelCreating), BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(entityModelDefiner != null, nameof(entityModelDefiner) + " != null");
                var action = (Action<ModelBuilder>)Delegate.CreateDelegate(typeof(Action<ModelBuilder>), null, entityModelDefiner);
                action(modelBuilder);
            }
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<State> States { get; set; }
    }
}
