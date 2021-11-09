using idb.Backend.Attributes;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using Markdig;
using Markdig.SyntaxHighlighting.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IItemRepository _itemRespository;

        public IDBController(ILogger<IDBController> logger, IUserRepository userRepository, ITagRepository tagRepository, IItemRepository itemRespository)
        {
            _logger = logger;
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _itemRespository = itemRespository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.Get();
            var usersDTO = users.ToList().Select(user => new UserResponse(id: user.ID, guid: user.guid, email: user.email,
                first_name: user.first_name, last_name: user.last_name, joined_at: user.created_at, is_admin: user.is_admin)).ToList();
            return new OkObjectResult(usersDTO);
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.Get();
            var tagsDTO = tags.ToList().Select(tags => new TagsResponse(id: tags.ID, name: tags.name)).ToList();
            return new OkObjectResult(tagsDTO);
        }

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] PostTagsRequest tag)
        {
            await _tagRepository.Create(new Tag { name = tag.name, ID = tag.id });
            return new OkResult();
        }


        [HttpGet("items")]
        public async Task<IActionResult> GetItems([FromQuery] string search, [FromQuery] string tag_ids, [FromQuery] int? user_id)
        {
            User user = null;
            var tagIds = string.IsNullOrEmpty(tag_ids) && string.IsNullOrWhiteSpace(tag_ids) ? new List<int>(0) : tag_ids.Split().Select(x => int.Parse(x)).ToList();

            if (user_id is not null)
                user = await _userRepository.GetByIDThatIWillDeleteSoon((int)user_id);

            var items = await _itemRespository.GetBy(search, tagIds, user?.guid ?? string.Empty);

            return new OkObjectResult(items.Select(x => new Item(id: x.ID,
                guid: x.guid,
                name: x.name,
                tags: x.tags.Select(x => new TagsResponse(x.ID, x.name)).ToList(),
                content: x.content,
                content_html: x.content_html,
                created_at: x.created_at))
                .ToList());

        }

        [HttpGet("items/{itemId}")]
        public async Task<IActionResult> GetItem(string itemId)
        {
            var item = await _itemRespository.Get(itemId);
            return new OkObjectResult(item);
        }

        [HttpPost("items")]
        public async Task<IActionResult> GetItems([FromBody] ItemPostRequest itemPost)
        {
            var userId = HttpContext.Items["userId"] as string;
            var itemTags = await _tagRepository.GetByIds(itemPost.tag_ids);
            var newItem = new DataAccess.Models.Item
            {
                ID = 1,
                tags = itemTags,
                name = itemPost.name,
                ownerId = userId,
                content = itemPost.content,
                content_html = Markdown.ToHtml(itemPost.content, _pipeline)

            };
            await _itemRespository.Create(newItem);
            return new OkObjectResult(newItem);
        }

        [HttpPatch("items/{itemId}")]
        public async Task<IActionResult> PatchItems(string itemId, [FromBody] ItemsPatchRequest patch)
        {
            var item = await _itemRespository.Get(itemId);
            var tags = await _tagRepository.GetByIds(patch.tag_ids);

            item.content = patch.content;
            item.content_html = Markdown.ToHtml(patch.content, _pipeline);
            item.tags = tags;
            await _itemRespository.Update(item);
            return new OkObjectResult(new Item(
                item.ID,
                item.guid,
                item.name,
                item.tags.Select(x => new TagsResponse(x.ID, x.name)).ToList(),
                item.content,
                item.content_html,
                item.created_at
            ));
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            await _itemRespository.Delete(itemId);
            return new OkObjectResult(null);
        }
    }

    public record TagsResponse(int id, string name);
    public record PostTagsRequest(int id, string name);
    public record Item(int id, string guid, string name, List<TagsResponse> tags, string content, string content_html, DateTime? created_at);
    public record ItemPostRequest(string name, string content, List<int> tag_ids);
    public record ItemsPatchRequest(string content, List<int> tag_ids);
}
