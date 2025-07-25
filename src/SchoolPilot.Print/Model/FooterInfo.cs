

namespace SchoolPilot.Print.Model
{
    public class FooterInfo
    {
        public string PatientName { get; set; }

        public string PatientDateOfBirth { get; set; }

        public string PatientGender { get; set; }

        public List<BasicPrintSignature> Signatures { get; set; }
    }

    public class BasicPrintSignature
    {
        public string Name { get; set; }

        public string Credentials { get; set; }

        //public SignatureType Type { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
    }
}
