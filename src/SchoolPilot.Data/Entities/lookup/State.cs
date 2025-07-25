

namespace SchoolPilot.Data.Entities.lookup
{
    public class State
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public virtual Country Country { get; set; }

        public string? Region { get; set; } // Geopolitical zone for Nigerian states
        public bool IsNigerianState { get; set; }
    }
}
