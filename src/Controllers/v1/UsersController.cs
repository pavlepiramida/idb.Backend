using idb.Backend.Attributes;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace idb.Backend.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/idb/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.Get();
            var usersDTO = users.ConvertAll(user => new UserResponse(
                id: user.ID,
                guid: user.guid,
                email: user.email,
                first_name: user.first_name,
                last_name: user.last_name,
                joined_at: user.created_at,
                is_admin: user.is_admin));
            return new OkObjectResult(usersDTO);
        }
    }
}
