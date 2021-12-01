using idb.Backend.Attributes;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace idb.Backend.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Items["userId"] as string;
            var user = await _userRepository.Get(userId);

            return new OkObjectResult(new UserResponse(
                id: user.ID,
                guid: user.guid,
                email: user.email,
                first_name: user.first_name,
                last_name: user.last_name,
                joined_at: user.created_at,
                is_admin: user.is_admin));
        }
    }
}