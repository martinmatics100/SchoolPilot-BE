

using SchoolPilot.Data.Interfaces;

namespace SchoolPilot.Data.Entities.Users
{
    public class AppUserRole : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
