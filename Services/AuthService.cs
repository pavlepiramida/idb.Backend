using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace idb.Backend.Services
{
    public class AuthService
    {
        public string GenerateJwt(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(EnvConsts.JWT_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", userId) }),
                Expires = DateTime.UtcNow.AddMinutes(EnvConsts.JWT_LIFETIME_MINUTES),
                Issuer = EnvConsts.JWT_ISSUER,
                Audience = EnvConsts.JWT_AUDIENCE,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}