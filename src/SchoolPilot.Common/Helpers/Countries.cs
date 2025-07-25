

using ISO3166;
using NLog;
using PhoneNumbers;

namespace SchoolPilot.Common.Helpers
{
    public class Countries
    {
        private static readonly IDictionary<string, Country> BasicCache;

        private static readonly IDictionary<string, CountryWithPhone> PhoneCache;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly HashSet<string> Territories = new HashSet<string>()
        {
            "850", //Virgin Islands us
            "016", //American samoa
            "580", //Northern Mariana Islands
            "630", //Puerto Rico,
            "316", //Guam
        };

        private static readonly PhoneNumberUtil PhoneNumberUtil;

        public const string Nigeria = "566";

        static Countries()
        {
            BasicCache = Country.List
                .ToDictionary(k => k.NumericCode);

            PhoneNumberUtil = PhoneNumberUtil.GetInstance();

            PhoneCache = Country.List
                .Select(s =>
                {
                    var countryCodeForRegion = PhoneNumberUtil.GetCountryCodeForRegion(s.TwoLetterCode);
                    string fullCode;
                    switch (countryCodeForRegion)
                    {
                        /*
                         * Several countries or regions are so small that they do not have a country code:
                         * Antarctica
                         * Bouvet Island
                         * French Southern Territories
                         * Heard Island and McDonald Islands
                         * Pitcairn
                         * South Georgia and the South Sandwich Islands
                         * United States Minor Outlying Islands
                         */
                        case 0:
                            return null;
                        case 1:
                            {
                                var metadata = PhoneNumberUtil.GetMetadataForRegion(s.TwoLetterCode);
                                fullCode = countryCodeForRegion.ToString();
                                if (!string.IsNullOrEmpty(metadata.LeadingDigits))
                                {
                                    fullCode += "-" + metadata.LeadingDigits;
                                }
                                break;
                            }
                        default:
                            fullCode = countryCodeForRegion.ToString();
                            break;
                    }

                    return new CountryWithPhone
                    {
                        Name = s.Name,
                        NumericCode = s.NumericCode,
                        TwoLetterCode = s.TwoLetterCode,
                        CountryCode = fullCode
                    };
                })
                //
                .Where(w => w != null)
                .ToDictionary(k => k.NumericCode);
        }

        public static IDictionary<string, Country> Lookup()
        {
            return BasicCache;
        }

        public static IDictionary<string, CountryWithPhone> LookupPhone()
        {
            return PhoneCache;
        }

        public static string GetCountryName(string numericCode)
        {
            if (BasicCache.TryGetValue(numericCode, out var value))
            {
                return value.Name;
            }
            Logger.Fatal($"The numeric country code {numericCode} does not exist within ISO-3166");
            return "N/A";
        }

        public static string FormatInternationalPhoneNumber(string destinationNumericCode, string phoneNumber, string originNumericCode = Nigeria)
        {
            if (string.IsNullOrEmpty(destinationNumericCode))
            {
                destinationNumericCode = Nigeria;
            }
            if (PhoneCache.TryGetValue(destinationNumericCode, out var value))
            {
                var parsedNumber = PhoneNumberUtil.Parse(phoneNumber, value.TwoLetterCode);

                //The origin numeric code can be the code of the location (When countries is added),
                //so that if the international country is the same as the location it would be using the national phone format instead.
                var format = originNumericCode == destinationNumericCode
                    ? PhoneNumberFormat.NATIONAL
                    : PhoneNumberFormat.INTERNATIONAL;

                return PhoneNumberUtil.Format(parsedNumber, format);
            }
            Logger.Fatal($"The numeric country code {destinationNumericCode} does not exist within ISO-3166");
            return phoneNumber;
        }

        public static string ConvertTerritoryToState(string country)
        {
            switch (country)
            {
                case "850":  //Virgin Islands us
                    return "VI";
                case "016": //American samoa
                    return "AS";
                case "630": //Puerto Rico,
                    return "PR";
                case "316": //Guam
                    return "GU";
                case "580": //Northern Mariana Islands
                    return "MP";
            }
            throw new ArgumentOutOfRangeException(nameof(country), "Country is not a valid territory code.");
        }

    }


    public class CountryWithPhone
    {
        public string NumericCode { get; set; }

        public string CountryCode { get; set; }

        public string TwoLetterCode { get; set; }

        public string Name { get; set; }
    }
}
