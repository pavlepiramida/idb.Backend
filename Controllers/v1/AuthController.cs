using System.Threading.Tasks;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using idb.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace idb.Backend.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILogger<AuthController> _logger;
        private AuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(ILogger<AuthController> logger, AuthService authService, IUserRepository userRepository)
        {
            _logger = logger;
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest login)
        {
            var user = await _userRepository.GetByEmail(login.Email);
            if (user is null)
                return new NotFoundResult();

            if (user.password != login.Password)
                return new BadRequestResult();

            var token = _authService.GenerateJwt(user.guid);


            return new OkObjectResult(new {token});
        }
    }
}