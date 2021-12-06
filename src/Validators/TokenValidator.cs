using idb.Backend.Providers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace idb.Backend.Validators
{
    public interface IJwtTokenValidator
    {
        bool TryValidateJwtToken(string token, out JwtSecurityToken validatedToken);
    }

    public class TokenValidator : IJwtTokenValidator
    {
        private readonly IJwtEnvironmentProvider _jwtEnv;

        public TokenValidator(IJwtEnvironmentProvider jwtEnv)
        {
            _jwtEnv = jwtEnv;
        }

        public bool TryValidateJwtToken(string token, out JwtSecurityToken validatedToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtEnv.JwtKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _jwtEnv.JwtIssuer,
                    ValidAudience = _jwtEnv.JwtAudience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = _jwtEnv.JwtValidateLifeTime,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken jwtToken);

                validatedToken = (JwtSecurityToken)jwtToken;
                return true;

            }
            catch
            {
                validatedToken = null;
                return false;
            }
        }
    }
}