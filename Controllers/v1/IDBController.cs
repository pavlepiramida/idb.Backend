using idb.Backend.Attributes;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Markdig;
using Markdig.SyntaxHighlighting.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace idb.Backend.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IDBController : ControllerBase
    {
        private readonly ILogger<IDBController> _logger;
        private readonly MarkdownPipeline _pipeline;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IItemRepository _itemRepository;

        public IDBController(ILogger<IDBController> logger, IUserRepository userRepository,
            ITagRepository tagRepository, IItemRepository itemRepository)
        {
            _logger = logger;
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _itemRepository = itemRepository;
        }

        [HttpGet("users")]
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

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.Get();
            var tagsDTO = tags.ConvertAll(tag => new TagsResponse(id: tag.ID, name: tag.name));
            return new OkObjectResult(tagsDTO);
        }

        [HttpPost("tags/{tag}")]
        public async Task<IActionResult> CreateTag([FromRoute] string tag)
        {
            await _tagRepository.Create(new Tag { name = tag });
            return new OkResult();
        }


        [HttpGet("items")]
        public async Task<IActionResult> GetItems([FromQuery] string search, [FromQuery] string tag_ids,
            [FromQuery] int? user_id)
        {
            User user = null;
            var tagIds = string.IsNullOrEmpty(tag_ids) && string.IsNullOrWhiteSpace(tag_ids)
                ? new List<int>(0)
                : tag_ids.Split(",").Select(int.Parse).ToList();

            if (user_id is not null)
                user = await _userRepository.GetByIDThatIWillDeleteSoon((int)user_id);

            var items = await _itemRepository.GetBy(search, tagIds, user?.guid ?? string.Empty);

            return new OkObjectResult(items.ConvertAll(item => new ItemResponse(
                    id: item.ID,
                    guid: item.guid,
                    name: item.name,
                    tags: item.tags.ConvertAll(tag => new TagsResponse(tag.ID, tag.name)),
                    content: item.content,
                    content_html: item.content_html,
                    created_at: item.created_at))
);
        }

        [HttpGet("items/{itemId}")]
        public async Task<IActionResult> GetItem(string itemId)
        {
            var item = await _itemRepository.Get(itemId);
            return new OkObjectResult(item);
        }

        [HttpPost("items")]
        public async Task<IActionResult> GetItems([FromBody] ItemPostRequest itemPost)
        {
            var userId = HttpContext.Items["userId"] as string;
            var itemTags = await _tagRepository.GetByIds(itemPost.tag_ids);
            var newItem = new Item
            {
                tags = itemTags,
                name = itemPost.name,
                ownerId = userId,
                content = itemPost.content,
                content_html = Markdown.ToHtml(itemPost.content, _pipeline)
            };
            await _itemRepository.Create(newItem);
            return new OkObjectResult(newItem);
        }

        [HttpPatch("items/{itemId}")]
        public async Task<IActionResult> PatchItems(string itemId, [FromBody] ItemsPatchRequest patch)
        {
            var item = await _itemRepository.Get(itemId);
            var tags = await _tagRepository.GetByIds(patch.tag_ids);

            item.content = patch.content;
            item.content_html = Markdown.ToHtml(patch.content, _pipeline);
            item.tags = tags;
            await _itemRepository.Update(item);
            return new OkObjectResult(new ItemResponse(
                id: item.ID,
                guid: item.guid,
                name: item.name,
                tags: item.tags.ConvertAll(x => new TagsResponse(x.ID, x.name)),
                content: item.content,
                content_html: item.content_html,
                created_at: item.created_at
            ));
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            await _itemRepository.Delete(itemId);
            return new OkObjectResult(null);
        }
    }
}