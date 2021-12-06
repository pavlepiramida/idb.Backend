using idb.Backend.Providers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace idb.Backend.Services
{
    public interface IAuthJwtService
    {
        string GenerateJwt(string userId);
    }

    public class AuthService : IAuthJwtService
    {
        private readonly IJwtEnvironmentProvider _jwtEnv;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthService(IJwtEnvironmentProvider jwtEnv, IDateTimeProvider dateTimeProvider)
        {
            _jwtEnv = jwtEnv;
            _dateTimeProvider = dateTimeProvider;
        }

        public string GenerateJwt(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtEnv.JwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", userId) }),
                Expires = _dateTimeProvider.UtcNow.AddMinutes(_jwtEnv.JwtLifeTimeMinutes),
                Issuer = _jwtEnv.JwtIssuer,
                Audience = _jwtEnv.JwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}