

using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Model;

namespace SchoolPilot.Common.Constants
{
    public static class SubjectDefinitionsConstants
    {
        public static readonly List<SubjectDefinition> All = new()
        {
            new()
            {
                Name = "Mathematics",
                Code = "MTH",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryCore }
            },
            new()
            {
                Name = "English Language",
                Code = "ENG",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryCore }
            },
            new()
            {
                Name = "Basic Science",
                Code = "B.S",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Basic Technology",
                Code = "B-TECH",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Information Technology",
                Code = "I.C.T",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryCore },
            },

            new()
            {
                Name = "Physical and Health Education",
                Code = "P.H.E",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary },
            },
            new()
            {
                Name = "Civic Education",
                Code = "Civic-Edu",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryCore }
            },
            new()
            {
                Name = "Cultural and Creative Arts",
                Code = "C.C.A",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Social Studies",
                Code = "Social-Stud",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Christian Religious Studies",
                Code = "C.R.S",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Islamic Religious Studies",
                Code = "I.R.S",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Security Education",
                Code = "Security Edu",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "National Values Education",
                Code = "N.V.E",
                Levels = new[] { SchoolLevel.Primary },
                Category = new[] {SubjectCategory.IsPrimary }
            },
            new()
            {
                Name = "Agricultural Science",
                Code = "AGRIC",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
            new()
            {
                Name = "Igbo Language",
                Code = "Igbo",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Yoruba Language",
                Code = "Yoruba",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Hausa Language",
                Code = "Hausa",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "French Language",
                Code = "French",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryArtsStreamOptional }
            },
            new()
            {
                Name = "Arabic Language",
                Code = "Arabic",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Home Economics",
                Code = "Home Econs",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Entrepreneurship ",
                Code = "Entrepreneurship",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary, SchoolLevel.SeniorSecondary  },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary, SubjectCategory.IsSeniorSecondaryCore }
            },
            new()
            {
                Name = "History ",
                Code = "HIS",
                Levels = new[] { SchoolLevel.Primary, SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsPrimary, SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Business Studies",
                Code = "Bus Stud",
                Levels = new[] { SchoolLevel.JuniorSecondary },
                Category = new[] {SubjectCategory.IsJuniorSecondary }
            },
            new()
            {
                Name = "Biology",
                Code = "BIO",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamCore }
            },
            new()
            {
                Name = "Physics",
                Code = "PHY",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamCore }
            },
            new()
            {
                Name = "Chemistry",
                Code = "CHEM",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamCore }
            },
            new()
            {
                Name = "Further Mathematics",
                Code = "F-MATH",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
            new()
            {
                Name = "Literature in English",
                Code = "LIT-IN-ENG",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Government",
                Code = "GOVT",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryArtsStreamCore }
            },
            new()
            {
                Name = "Economics",
                Code = "ECONS",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] { SubjectCategory.IsSeniorSecondaryScienceStreamOptional, SubjectCategory.IsSeniorSecondaryArtsStreamCore, SubjectCategory.IsSeniorSecondaryCommercialStreamCore }
            },
            new()
            {
                Name = "History",
                Code = "His",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryArtsStreamOptional }
            },
            new()
            {
                Name = "Commerce",
                Code = "COMM",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryCommercialStreamCore }
            },
            new()
            {
                Name = "Financial Accounting",
                Code = "FIN-ACCT",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryCommercialStreamCore }
            },
            new()
            {
                Name = "Office Practice",
                Code = "OFF-PRACT",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryCommercialStreamCore }
            },
            new()
            {
                Name = "Store Management",
                Code = "STORE-MGMT",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryCommercialStreamCore }
            },
            new()
            {
                Name = "Geography",
                Code = "GEO",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
            new()
            {
                Name = "Food and Nutrition",
                Code = "Food and Nut",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
            new()
            {
                Name = "Visual Arts",
                Code = "Visual Arts",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
            new()
            {
                Name = "Technical Drawing",
                Code = "TDD",
                Levels = new[] { SchoolLevel.SeniorSecondary },
                Category = new[] {SubjectCategory.IsSeniorSecondaryScienceStreamOptional }
            },
        };
    }
}
