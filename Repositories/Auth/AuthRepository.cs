
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;

namespace XeniaRegistrationBackend.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
       

        public AuthRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region ADMIN
        public async Task<UserAuthResponseDto?> AuthenticateUser(LoginRequestDto request)
        {
            var user = await _context.Users
                .Where(u => u.UserName == request.UserName)               
                .FirstOrDefaultAsync();

            if (user == null)
                throw new UnauthorizedAccessException("User not found.");


            if (user.Password != request.Password)
                throw new UnauthorizedAccessException("Incorrect password.");

            var response = new UserAuthResponseDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Role = user.Role,
            };

            return response;
        }

       

        public string GenerateJwtToken(UserAuthResponseDto user)
        {
            var keyString = _configuration["JwtSettings:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var issuer = _configuration["JwtSettings:Issuer"]
                ?? throw new InvalidOperationException("JWT issuer is not configured.");

            var audience = _configuration["JwtSettings:Audience"]
                ?? throw new InvalidOperationException("JWT audience is not configured.");

            var expirationMinutesString = _configuration["JwtSettings:ExpirationMinutes"]
                ?? throw new InvalidOperationException("JWT expiration is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("UserType", user.Role.ToString() ?? "0"),                
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expirationMinutesString)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        #endregion
    }
}
