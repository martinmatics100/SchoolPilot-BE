

namespace SchoolPilot.Export.Enums
{
    public enum ExportDataFormat
    {
        PropertyType,
        Enum,
        /// <summary>
        /// Show enum's value as N/A if the field is null.
        /// </summary>
        EnumOrNotAvailable,
        Decimal1,
        Decimal2,
        Decimal3,
        Decimal4,
        Currency,
        NoZeroNumber,
        ZipCode,
        PhoneNumber,
        DateWithConditionalTime,
        Custom,
        Time,
        GeneralNumber,
        DateOrNotAvailable,
        DateAndTime,
        Country,
        FlaggedEnum
    }
}
