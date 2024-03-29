﻿using idb.Backend.Attributes;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Markdig;
using Markdig.SyntaxHighlighting.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace idb.Backend.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/idb/[controller]")]
    public class ItemsController : ControllerBase
    {
        private const string itemTemplate = "{itemId}";
        private readonly MarkdownPipeline _pipeline;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;

        public ItemsController(IItemRepository itemRepository, IUserRepository userRepository, ITagRepository tagRepository)
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
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

            return items is null ? new NotFoundResult() : new OkObjectResult(items.ConvertAll(item =>
            new ItemResponse(
                id: item.ID,
                guid: item.guid,
                name: item.name,
                tags: item.tags.ConvertAll(tag => new TagsResponse(tag.ID, tag.name)),
                content: item.content,
                content_html: Markdown.ToHtml(item.content, _pipeline),
                created_at: item.created_at)));
        }

        [HttpGet(itemTemplate)]
        public async Task<IActionResult> GetItem(string itemId)
        {
            var item = await _itemRepository.Get(itemId);
            return item is null ? new NotFoundResult() :
                new OkObjectResult(new ItemResponse(
                    id: item.ID,
                    guid: item.guid,
                    name: item.name,
                    tags: item.tags.ConvertAll(x => new TagsResponse(x.ID, x.name)),
                    content: item.content,
                    content_html: Markdown.ToHtml(item.content, _pipeline),
                    created_at: item.created_at));
        }

        [HttpPost]
        public async Task<IActionResult> PostItems([FromBody] ItemPostRequest itemPost)
        {
            var userId = HttpContext.Items["userId"] as string;
            var itemTags = await _tagRepository.GetByIds(itemPost.tag_ids);
            var newItem = new Item
            {
                tags = itemTags,
                name = itemPost.name,
                ownerId = userId,
                content = itemPost.content
            };
            var createdItem = await _itemRepository.Create(newItem);
            return new OkObjectResult(createdItem);
        }

        [HttpPatch(itemTemplate)]
        public async Task<IActionResult> PatchItems(string itemId, [FromBody] ItemsPatchRequest patch)
        {
            var item = await _itemRepository.Get(itemId);
            var tags = await _tagRepository.GetByIds(patch.tag_ids);

            item.content = patch.content;
            item.tags = tags;
            var updatedItem = await _itemRepository.Update(item);
            return new OkObjectResult(new ItemResponse(
                id: updatedItem.ID,
                guid: updatedItem.guid,
                name: updatedItem.name,
                tags: updatedItem.tags.ConvertAll(x => new TagsResponse(x.ID, x.name)),
                content: updatedItem.content,
                content_html: Markdown.ToHtml(updatedItem.content, _pipeline),
                created_at: updatedItem.created_at
            ));
        }

        [HttpDelete(itemTemplate)]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            await _itemRepository.Delete(itemId);
            return new OkObjectResult(null);
        }
    }
}
