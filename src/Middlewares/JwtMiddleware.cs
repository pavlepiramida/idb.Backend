using idb.Backend.Validators;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace idb.Backend.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtTokenValidator validator)
        {
            var token = context.Request.Headers["Authorization"].ToString()?.Split(" ").Last();

            if (validator.TryValidateJwtToken(token, out var validatedToken))
                context.Items["userId"] = validatedToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;

            await _next(context);
        }
    }
}