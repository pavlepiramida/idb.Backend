using idb.Backend.Attributes;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace idb.Backend.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/idb/[controller]")]
    public class TagsController : ControllerBase
    {
        private const string tagTemplate = "{tag}";
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.Get();
            return new OkObjectResult(tags.ConvertAll(tag => new TagsResponse(id: tag.ID, name: tag.name)));
        }

        [HttpPost(tagTemplate)]
        public async Task<IActionResult> PostTag([FromRoute] string tag)
        {
            await _tagRepository.Create(new Tag { name = tag });
            return new OkResult();
        }
    }
}
