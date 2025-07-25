

using SchoolPilot.Data.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Helpers;
using System.ComponentModel;
using SchoolPilot.Common.Extensions;

namespace SchoolPilot.Data.Entities
{

    /// <summary>
    /// Base PhoneNumber class, extended by other entities, so that they can add their own implementation of it.
    /// The JsonIgnores are so ObjectDiffPatch can be used to compare against models and see if there are changes.
    /// </summary>
    public abstract class PhoneNumber : ModeledEntity, IEntity, IAuditable, IAccountScope, IPhoneNumber
    {
        protected PhoneNumber()
        {
            // Set a default, so that the other addresses that are not setting
            // it yet won't fail due to the field being not nullable in the db
            Country = Countries.Nigeria;
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid AccountId { get; set; }

        [MaxLength(15)]
        public string Number { get; set; }

        [MaxLength(5)]
        public string Extension { get; set; }

        public PhoneType Type { get; set; }

        public bool IsPrimary { get; set; }

        [MinLength(3)]
        [MaxLength(3)]
        [DefaultValue(Countries.Nigeria)]
        [Required]
        public string Country { get; set; }

        [JsonIgnore]
        public DateTime? CreatedOn { get; set; }

        [JsonIgnore]
        public DateTime? ModifiedOn { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Number))
            {
                return string.Empty;
            }

            return Number + Extension;
        }

        public string Format()
        {
            if(string.IsNullOrWhiteSpace(Number))
            {
                return string.Empty;
            }

            var result = Countries.FormatInternationalPhoneNumber(Country, Number);
            if (!Extension.IsNullOrEmpty())
            {
                result += " x" + Extension;
            }

            return result;
        }
    }
}
