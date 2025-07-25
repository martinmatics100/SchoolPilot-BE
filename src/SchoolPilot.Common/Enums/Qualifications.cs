

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum Qualifications
    {
        [Description("National Certificate in Education")]
        NCE = 1,
        [Description("Ordinary National Diploma")]
        OND = 2,
        [Description("Higher National Diploma")]
        HND = 3,
        [Description("Bachelor of Education")]
        BED = 4,
        [Description("Bachelor of Science")]
        BSC = 5,
        [Description("Bachelor of Arts")]
        BA = 6,
        [Description("Master of Education")]
        MED = 7,
        [Description("Master of Science")]
        MSC = 8,
        [Description("Master of Arts")]
        MA = 9,
        [Description("Doctor of Philosophy")]
        PHD = 10,
        [Description("Postgraduate Diploma in Education")]
        PGDE = 11,
        [Description("Technical or Vocational Certificate")]
        TVC = 12,
        [Description("Professional Certificate")]
        CERTIFICATE = 13
    }
}
