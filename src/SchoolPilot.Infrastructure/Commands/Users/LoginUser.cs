

using MediatR;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;
using SchoolPilot.Data.Context;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Entities;
using SchoolPilot.Business.BasicResults;
using System.Net;
using SchoolPilot.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Data.Helpers;
using SchoolPilot.Infrastructure.Helpers;

namespace SchoolPilot.Infrastructure.Commands.Users
{
    public static class LoginUser
    {
        public class Request : IRequest<Result>
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public class Result : IActionResult
        {
            public HttpStatusCode Status { get; set; }
            public string ErrorMessage { get; set; }
            public string AccessToken { get; set; }
            public string? TokenType { get; set; }

            public DateTime ExpiresIn { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ReadWriteSchoolPilotContext _readWriteContext;
            private readonly ITokenService _tokenService;
            private readonly IConfiguration _configuration;
            private readonly IPasswordHasher _passwordHasher;

            public Handler(ReadWriteSchoolPilotContext readWriteContext, ITokenService tokenService, IConfiguration configuration, IPasswordHasher passwordHasher)
            {
                _readWriteContext = readWriteContext;
                _tokenService = tokenService;
                _configuration = configuration;
                _passwordHasher = passwordHasher;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new Result
                    {
                        Status = HttpStatusCode.BadRequest,
                        ErrorMessage = "Email and password are required"
                    };
                }

                var user = await _readWriteContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (user == null || !_passwordHasher.VerifyPassword(user.Password, request.Password))
                {
                    return new Result
                    {
                        Status = HttpStatusCode.Unauthorized,
                        ErrorMessage = "Invalid credentials"
                    };
                }

                var token = await _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                if(user.LoginId == Guid.Empty)
                {
                    user.LastValidated = DateTime.UtcNow;
                    user.LoginId = SequentialGuid.Create();
                }

                // Handle refresh token (separate table)
                var existingRefreshToken = await _readWriteContext.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.UserId == user.Id, cancellationToken);

                if (existingRefreshToken == null)
                {
                    // Create new refresh token
                    _readWriteContext.RefreshTokens.Add(new RefreshToken
                    {
                        Id = SequentialGuid.Create(),
                        Token = refreshToken,
                        ExpiryTime = DateTime.UtcNow.AddDays(30), // Example expiration
                        UserId = user.Id,
                        //Created = DateTime.UtcNow
                    });
                }
                else
                {
                    // Update existing refresh token
                    existingRefreshToken.Token = refreshToken;
                    existingRefreshToken.ExpiryTime = DateTime.UtcNow.AddDays(30);
                    existingRefreshToken.IsRevoked = false;
                }

                await _readWriteContext.SaveChangesAsync(cancellationToken);

                // Get token expiration from configuration
                var expiresInMinutes = _configuration.GetValue<double>("JwtSettings:Expires");


                return new Result
                {
                    Status = HttpStatusCode.OK,
                    AccessToken = token,
                    ExpiresIn = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                    TokenType = "Bearer"
                };
            }
        }
    }
}
