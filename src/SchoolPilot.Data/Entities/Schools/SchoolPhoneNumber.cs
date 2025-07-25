

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPilot.Data.Entities.Schools
{

    /// <summary>
    /// School's usage of the phone number.
    /// A separate class is used, so that a foreign key can be setup.
    /// </summary>
    [UsedImplicitly]
    [Table("SchoolPhoneNumber")]
    public class SchoolPhoneNumber : PhoneNumber
    {

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchoolPhoneNumber>()
                .AddIndex(builder => builder
                    .HasIndex(patientPayor => patientPayor.Type))
                .AddIndex(builder => builder.HasIndex(p => p.AccountId).HasDatabaseName("Idx_SchoolPhoneNo_AccId"));
        }
    }
}
