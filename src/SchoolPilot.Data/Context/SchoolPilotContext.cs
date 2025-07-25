

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Entities.Logs;
using SchoolPilot.Data.Entities.Schools;
using SchoolPilot.Data.Entities.Staffs;
using SchoolPilot.Data.Entities.Students;
using SchoolPilot.Data.Entities.Users;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace SchoolPilot.Data.Context
{
    public abstract class SchoolPilotContext : DbContext
    {
        public string? ConnectionStringName { get; }

        protected SchoolPilotContext() { }

        protected SchoolPilotContext(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        protected SchoolPilotContext(DbContextOptions<SchoolPilotContext> contextOptions) : base(contextOptions) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<ProviderFeatureSetting> ProviderFeatures { get; set; }

        public DbSet<AppUserRole> AppUserRoles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<BaseSchool> Schools { get; set; }

        public DbSet<UserDefaultSchool> UserDefaultSchools { get; set; }

        public DbSet<SchoolSetting> SchoolSettings { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<ActivityLogDetails> ActivityLogDetails { get; set; }

        public DbSet<SchoolPhoneNumber> SchoolPhoneNumbers { get; set; }

        public DbSet<NewPhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<UserAffiliation> UserAffiliations { get; set; }

        public DbSet<UserAffiliationHistory> UserAffiliationsHistories { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserPrinciple> UserPrinciples { get; set; }

        public DbSet<Principle> Principles { get; set; }

        public DbSet<SchoolLocation> SchoolLocations { get; set; }

        public DbSet<UserLocation> UserLocations { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<StoredFilesMetadata> StoredFilesMetadatas { get; set; }

        public DbSet<SchoolSubject> SchoolSubjects { get; set; }

        public DbSet<Student> Students { get; set; }


        public override int SaveChanges()
        {
            var entities = (from entry in ChangeTracker.Entries()
                            where entry.State == EntityState.Modified || entry.State == EntityState.Added
                            select entry.Entity);

            var validationResults = new List<DbEntityValidationResult>();

            foreach (var entity in entities)
            {
                ICollection<ValidationResult> entityValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(entity, new ValidationContext(entity), entityValidationResults))
                {
                    validationResults.Add(new DbEntityValidationResult(entity, entityValidationResults));
                }
            }

            if (!validationResults.IsNullOrEmpty())
            {
                throw new DbEntityValidationException(validationResults);
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Modified || entry.State == EntityState.Added)
                .ToList();

            var validationResults = new List<DbEntityValidationResult>();

            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity.Entity);
                var entityValidationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(entity.Entity, validationContext, entityValidationResults, true))
                {
                    validationResults.Add(new DbEntityValidationResult(entity.Entity, entityValidationResults));
                }
            }

            if (validationResults.Any())
            {
                throw new DbEntityValidationException(validationResults);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static Func<object, object> MakeCastDelegate(Type from, Type to)
        {
            var p = Expression.Parameter(typeof(object)); // do not inline
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(Expression.ConvertChecked(Expression.Convert(p, from), to), typeof(object)),
                p).Compile();
        }

        private static readonly Dictionary<Tuple<Type, Type>, Func<object, object>> CastCache
            = new Dictionary<Tuple<Type, Type>, Func<object, object>>();

        public static Func<object, object> GetCastDelegate(Type from, Type to)
        {
            lock (CastCache)
            {
                var key = new Tuple<Type, Type>(from, to);
                Func<object, object> cast_delegate;
                if (!CastCache.TryGetValue(key, out cast_delegate))
                {
                    cast_delegate = MakeCastDelegate(from, to);
                    CastCache.Add(key, cast_delegate);
                }

                return cast_delegate;
            }
        }

        public static object Cast(Type t, object o)
        {
            return GetCastDelegate(o.GetType(), t).Invoke(o);
        }

        //Static LoggerFactory object
        public static readonly ILoggerFactory MyLoggerFactory =
            LoggerFactory.Create(
                builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //.ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model
                .GetEntityTypes()
                .Where(w => w.ClrType.IsSubclassOf(typeof(ModeledEntity)));

            foreach (var entity in entities)
            {
                var clrType = entity.ClrType;

                // Skip abstract classes
                if (clrType.IsAbstract) continue;

                var methodInfo = clrType.GetMethod(
                    nameof(ModeledEntity.OnModelCreating),
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (methodInfo != null)
                {
                    var instance = Activator.CreateInstance(clrType);
                    var action = (Action<ModelBuilder>)Delegate.CreateDelegate(typeof(Action<ModelBuilder>), instance, methodInfo);
                    action(modelBuilder);
                }
            }

        }

    }
}
