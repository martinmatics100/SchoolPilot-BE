

using SchoolPilot.Data.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using SchoolPilot.Common.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;

namespace SchoolPilot.Data.Entities
{
    public class Address : ModeledEntity, IEntity, IAccountScope
    {
        //Json Ignore is used so that object diff patch can compare this entity with other address objects
        [JsonIgnore]
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [MaxLength(100)]
        public string AddressLine1 { get; set; }

        [MaxLength(100)]
        public string AddressLine2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MinLength(2)]
        [MaxLength(100)]
        public string State { get; set; }

        [MinLength(2)]
        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string County { get; set; }

        [MinLength(3)]
        [MaxLength(3)]
        [DefaultValue(Countries.Nigeria)]
        [Required]
        public string Country { get; set; }

        [Column(TypeName = "decimal(8, 6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }

        public bool IsValidated { get; set; }

        public bool IsBypassed { get; set; }

        public Address()
        {
            //Set a default, so that the other addresses that are not setting
            //it yet won't fail due to the field being not nullable in the db.
            Country = Countries.Nigeria;
        }

        public string FormatFirstRow()
        {
            if (string.IsNullOrWhiteSpace(AddressLine2))
            {
                return AddressLine1;
            }

            return AddressLine1 + ", " + AddressLine2;
        }

        public string FormatSecondRow()
        {
            return City + ", " + State + " " + (ZipCode?.Length == 9 ? ZipCode.Insert(5, "-") : ZipCode);
        }

        public string FormatThirdRow(bool includeNewLine)
        {
            if (Country == null || Country == Countries.Nigeria)
            {
                return string.Empty;
            }
            return Countries.GetCountryName(Country) + (includeNewLine ? Environment.NewLine : string.Empty);
        }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .AddIndex(builder => builder
                    .HasIndex(address => address.AccountId));
        }
    }
}
