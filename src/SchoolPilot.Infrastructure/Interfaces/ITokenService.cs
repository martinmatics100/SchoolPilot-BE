

using SchoolPilot.Data.Entities;

namespace SchoolPilot.Infrastructure.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
        string GenerateRefreshToken();
    }
}
