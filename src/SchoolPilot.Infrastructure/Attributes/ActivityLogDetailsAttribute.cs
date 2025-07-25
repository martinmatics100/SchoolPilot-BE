

using SchoolPilot.Data.Entities.Logs;

namespace SchoolPilot.Infrastructure.Attributes
{
    public class ActivityLogDetailsAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public SubEntityType? SubEntityType { get; set; }

        public bool IsSingleField { get; set; }

        //This can be used for fields that should be recorded for debugging/records, but wouldn't need to be shown to the user.
        // Especially helpful with hard deleted item Ids, which could then be used to recover the object
        public bool IsHidden { get; set; }

        public string NullDefaultValue { get; set; }

        /// <summary>
        /// If nothing is passed, then it is assumed that the field is hidden. If no other parameters are needed, then no attribute is needed.
        /// </summary>
        public ActivityLogDetailsAttribute()
        {
            IsHidden = true;
            IsSingleField = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="displayName">What should be displayed to the user in the Activity Log Details.</param>
        public ActivityLogDetailsAttribute(string displayName)
        {
            DisplayName = displayName;
            IsSingleField = true;
        }

        public ActivityLogDetailsAttribute(int subEntityType, bool isSingleField = false, string displayName = null)
        {
            DisplayName = displayName;

            //Attribute constructors cannot have a nullable type since it does not know what the default value is at compile time.
            SubEntityType = (SubEntityType)subEntityType;
            IsSingleField = isSingleField;
        }
    }
}
