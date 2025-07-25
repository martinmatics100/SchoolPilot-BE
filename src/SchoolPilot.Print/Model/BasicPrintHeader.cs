

namespace SchoolPilot.Print.Model
{
    public class BasicPrintHeader
    {
        public string Title { get; set; }

        public string ProviderName { get; set; }

        public string AgencyAddressLine1 { get; set; }

        public string AgencyAddressLine2 { get; set; }

        public string AgencyPhoneNumber { get; set; }

        public string AgencyFaxNumber { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string ProviderNumber { get; set; }

        public int MarginTop { get; set; } = 65;

        public string OrderNumber { get; set; }

        public bool IsElectronicReferral { get; set; }

        public bool ExcludeHeader { get; set; }

        public string LogoUrl { get; set; }
    }
}
