

//using Microsoft.EntityFrameworkCore;
//using SchoolPilot.Common.Enums;
//using SchoolPilot.Data.Extensions;
//using SchoolPilot.Data.Interfaces;
//using System.ComponentModel.DataAnnotations;

//namespace SchoolPilot.Data.Entities.Schools
//{
//    public class School : BaseSchool, IContact<SchoolPhoneNumber>
//    {
//        public SchoolCategory Category { get; set; }
//        public SchoolOwnership SchoolOwnership { get; set; }
//        public SchoolType Type { get; set; }
//        public virtual Address SchoolPrimaryAddress { get; set; }
//        public virtual SchoolPhoneNumber ContactPersonPhone { get; set; }

//        public Guid? ModifiedBy { get; set; }

//        internal override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<School>()
//                .AddIndex(builder => builder.HasIndex(s => s.AccountId)
//                    .HasDatabaseName("IDX_School_AccountId"))
//                .AddIndex(builder => builder.HasIndex(s => s.LocationId)
//                    .HasDatabaseName("IDX_Patients_LocationId"));
//        }
//    }
//}
