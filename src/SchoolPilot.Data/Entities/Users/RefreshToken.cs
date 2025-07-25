

using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Users
{
    public class RefreshToken : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        public string Token { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
