using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPilot.Data.Entities.lookup
{
    public class Country
    {
        public Guid Id { get; set; }

        public string Code { get; set; } // ISO 2-letter country code (e.g., NG)

        public string Name { get; set; }

        public string? Region { get; set; } // (e.g., Africa)

        public string? Subregion { get; set; } // (e.g., Western Africa)

        public string? PhoneCode { get; set; } // International dialing code (e.g., +234)

        public string? Currency { get; set; } // Currency code (e.g., NGN)
    }
}
