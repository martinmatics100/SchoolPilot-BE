

using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Helpers;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SchoolPilot.Data.Entities
{
    /// Replacement for PhoneNumber class. The structure of doing a list of phone numbers for each entity has been difficult to maintain for both the front and backend.
    /// Thus we will start moving to using a single table of phone numbers, we need to slowly migrate to using this.
    /// IsPrimary is no longer needed, as entities should directly map a primary phone column to the primary number.
    /// The JsonIgnores are so ObjectDiffPatch can be used to compare against models and see if there are changes.
    [Table("PhoneNumbers")]
    public class NewPhoneNumber : IEntity, IAuditable, IAccountScope, IPhoneNumber
    {
        public NewPhoneNumber()
        {
            //Set a default, so that the other addresses that are not setting
            //it yet won't fail due to the field being not nullable in the db.
            Country = Countries.Nigeria;
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid AccountId { get; set; }

        [MaxLength(14)]
        public string Number { get; set; }

        [MaxLength(5)]
        public string Extension { get; set; }

        [MinLength(3)]
        [MaxLength(3)]
        [DefaultValue(Countries.Nigeria)]
        [Required]
        public string Country { get; set; }

        public PhoneType Type { get; set; }

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
            if (string.IsNullOrWhiteSpace(Number))
            {
                return string.Empty;
            }

            var number = Countries.FormatInternationalPhoneNumber(Country, Number);
            if (!string.IsNullOrWhiteSpace(Extension))
            {
                return number + " x" + Extension;
            }

            return number;
        }
    }
}
