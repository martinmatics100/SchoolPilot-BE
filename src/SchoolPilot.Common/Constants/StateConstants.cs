

namespace SchoolPilot.Common.Constants
{
    public static class StateConstants
    {
        public static class Nigeria
        {
            public const string Abia = "Abia State";
            public const string Adamawa = "Adamawa State";
            public const string AkwaIbom = "Akwa Ibom State";
            public const string Anambra = "Anambra State";
            public const string Bauchi = "Bauchi State";
            public const string Bayelsa = "Bayelsa State";
            public const string Benue = "Benue State";
            public const string Borno = "Borno";
            public const string CrossRiver = "Cross River State";
            public const string Delta = "Delta State";
            public const string Ebonyi = "Ebonyi State";
            public const string Edo = "Edo State";
            public const string Ekiti = "Ekiti State";
            public const string Enugu = "Enugu State";
            public const string Gombe = "Gombe State";
            public const string Imo = "Imo State";
            public const string Jigawa = "Jigawa State";
            public const string Kaduna = "Kaduna State";
            public const string Kano = "Kano State";
            public const string Katsina = "Katsina State";
            public const string Kebbi = "Kebbi State";
            public const string Kogi = "Kogi State";
            public const string Kwara = "Kwara State";
            public const string Lagos = "Lagos State";
            public const string Nasarawa = "Nasarawa State";
            public const string Niger = "Niger State";
            public const string Ogun = "Ogun State";
            public const string Ondo = "Ondo State";
            public const string Osun = "Osun State";
            public const string Oyo = "Oyo State";
            public const string Plateau = "Plateau State";
            public const string Rivers = "Rivers State";
            public const string Sokoto = "Sokoto State";
            public const string Taraba = "Taraba State";
            public const string Yobe = "Yobe State";
            public const string Zamfara = "Zamfara State";
            public const string FCT = "Federal Capital Territory";

            public static readonly Dictionary<string, string> StateCodes = new()
            {
                { Abia, "AB" },
                { Adamawa, "AD" },
                { AkwaIbom, "AK" },
                { Anambra, "AN" },
                { Bauchi, "BA" },
                { Bayelsa, "BY" },
                { Benue, "BE" },
                { Borno, "BO" },
                { CrossRiver, "CR" },
                { Delta, "DE" },
                { Ebonyi, "EB" },
                { Edo, "ED" },
                { Ekiti, "EK" },
                { Enugu, "EN" },
                { Gombe, "GO" },
                { Imo, "IM" },
                { Jigawa, "JI" },
                { Kaduna, "KD" },
                { Kano, "KN" },
                { Katsina, "KT" },
                { Kebbi, "KE" },
                { Kogi, "KO" },
                { Kwara, "KW" },
                { Lagos, "LA" },
                { Nasarawa, "NA" },
                { Niger, "NI" },
                { Ogun, "OG" },
                { Ondo, "ON" },
                { Osun, "OS" },
                { Oyo, "OY" },
                { Plateau, "PL" },
                { Rivers, "RI" },
                { Sokoto, "SO" },
                { Taraba, "TA" },
                { Yobe, "YO" },
                { Zamfara, "ZA" },
                { FCT, "FC" }
            };

            public static readonly List<string> AllStates = new()
            {
                Abia, Adamawa, AkwaIbom, Anambra, Bauchi, Bayelsa, Benue, Borno,
                CrossRiver, Delta, Ebonyi, Edo, Ekiti, Enugu, Gombe, Imo, Jigawa,
                Kaduna, Kano, Katsina, Kebbi, Kogi, Kwara, Lagos, Nasarawa, Niger,
                Ogun, Ondo, Osun, Oyo, Plateau, Rivers, Sokoto, Taraba, Yobe,
                Zamfara, FCT
            };
        }

        // Add other countries' states here when needed
        // public static class Ghana
        // {
        //     public const string GreaterAccra = "Greater Accra";
        //     ...
        // }
    }
}
