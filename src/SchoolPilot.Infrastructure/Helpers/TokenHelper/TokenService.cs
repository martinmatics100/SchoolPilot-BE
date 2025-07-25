

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SchoolPilot.Common.Constants;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities;
using SchoolPilot.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolPilot.Infrastructure.Helpers.TokenHelper
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _secretKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly ILogger<TokenService> _logger;
        private readonly ReadWriteSchoolPilotContext _readWriteContext;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, ReadWriteSchoolPilotContext context)
        {
            _logger = logger;
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _expires = jwtSettings.Expires;
            _readWriteContext = context;
        }

        public async Task<string> GenerateToken(User user)
        {
            var signingCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
            var claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(tokenOptions));
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private List<Claim> GetClaims(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var authTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.LoginId?.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.AuthTime, authTime.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("client_id", "school-pilot-frontend"), // Should come from configuration
                new Claim("scope", "trusted"),
                new Claim("subs", user?.LoginId?.ToString() ?? string.Empty),
                 //new Claim("sub", user.LoginId.ToString()),
                //new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                //new Claim("AccountId", user.SchoolAccountId.ToString() ?? string.Empty),
                //new Claim("FirstName", user.FirstName ?? string.Empty),
                //new Claim("LastName", user.LastName ?? string.Empty),
            };

            //// If you need additional role claims from the Roles table
            //var roles = _readWriteContext.Roles
            //    .Where(r => r.Name == user.Role)
            //    .ToList();

            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
            //}

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_expires),
                signingCredentials: signingCredentials
            );
        }
    }
}
