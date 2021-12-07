using idb.Backend.Controllers.v1;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace idb.Backend.Tests.Controllers.v1
{
    [TestFixture]
    public class IdbControllerTests
    {

        [Test]
        public async Task GetUsers_should_return_all_users()
        {
            var user = new User
            {
                ID = 1,
                guid = Guid.NewGuid().ToString("N"),
                created_at = DateTime.UtcNow,
                email = "pavle",
                password = "pavle",
                is_admin = true
            };
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get()).ReturnsAsync(new List<User> { user });
            var mockTagsRepository = new Mock<TagRepository>();
            var mockItemRepository = new Mock<ItemRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.GetUsers();
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as List<UserResponse>;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(user.ID, responseBody?[0].id);
            Assert.AreEqual(user.guid, responseBody?[0].guid);
            Assert.AreEqual(user.email, responseBody?[0].email);
        }

        [Test]
        public async Task GetTags_should_return_all_tags()
        {
            var mockTagsRepository = new Mock<ITagRepository>();
            var tag = new Tag
            {
                ID = 1,
                name = "tag",
                created_at = DateTime.Now
            };
            mockTagsRepository.Setup(x => x.Get()).ReturnsAsync(new List<Tag>() { tag });
            var mockUserRepository = new Mock<IUserRepository>();
            var mockItemRepository = new Mock<IItemRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.GetTags();
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as List<TagsResponse>;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(tag.ID, responseBody?[0].id);
            Assert.AreEqual(tag.name, responseBody?[0].name);
        }

        [Test]
        public async Task PostTag_should_return_200_OK()
        {
            var mockTagsRepository = new Mock<ITagRepository>();
            mockTagsRepository.Setup(x => x.Create(It.IsAny<Tag>())).Verifiable();
            var mockUserRepository = new Mock<IUserRepository>();
            var mockItemRepository = new Mock<IItemRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.PostTag("tagName");
            var okObject = (OkResult)response;
            var statusCode = okObject.StatusCode;

            Assert.AreEqual(200, statusCode);
            mockTagsRepository.Verify();
        }

        [Test]
        public async Task GetItems_should_return_200_OK_with_items()
        {
            var item = new Item
            {
                content = "**Test**",
                name = "Test",
                ownerId = Guid.NewGuid().ToString("N"),
                tags = new List<Tag>(),
                guid = Guid.NewGuid().ToString("N"),
                created_at = DateTime.Now
            };
            var mockItemRepository = new Mock<IItemRepository>();
            mockItemRepository.Setup(x => x.GetBy(It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<string>()))
                .ReturnsAsync(new List<Item> { item });
            var mockTagsRepository = new Mock<ITagRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.GetItems("searchTerm", "1", 1);
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as List<ItemResponse>;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(item.guid, responseBody?[0].guid);
            Assert.AreEqual(item.ID, responseBody?[0].id);
            Assert.AreEqual(item.content, responseBody?[0].content);
        }

        [Test]
        public async Task GetItem_should_return_200_OK_with_item()
        {
            var item = new Item
            {
                content = "**Test**",
                name = "Test",
                ownerId = Guid.NewGuid().ToString("N"),
                tags = new List<Tag>(),
                guid = Guid.NewGuid().ToString("N"),
                created_at = DateTime.Now
            };
            var mockItemRepository = new Mock<IItemRepository>();
            mockItemRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(item);
            var mockTagsRepository = new Mock<ITagRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.GetItem("long_ass_guid");
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as ItemResponse;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(item.guid, responseBody?.guid);
            Assert.AreEqual(item.ID, responseBody?.id);
            Assert.AreEqual(item.content, responseBody?.content);
        }

        [Test]
        public async Task PostItems_should_return_200_OK_with_created_item()
        {
            var itemRequest = new ItemPostRequest
            (
                content: "**Test**",
                name: "Test",
                tag_ids: new List<int> { 1 }
            );
            var tag = new Tag
            {
                ID = 1,
                name = "test"
            };
            var userId = Guid.NewGuid().ToString("N");
            var mockItemRepository = new Mock<IItemRepository>();
            mockItemRepository.Setup(x => x.Create(It.IsAny<Item>())).Verifiable();
            var mockTagsRepository = new Mock<ITagRepository>();
            mockTagsRepository.Setup(x => x.GetByIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag> { tag });
            var mockUserRepository = new Mock<IUserRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Items.Add("userId", userId);

            var response = await controller.PostItems(itemRequest);
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as Item;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(itemRequest.name, responseBody?.name);
            Assert.AreEqual(itemRequest.content, responseBody?.content);
            Assert.AreEqual(itemRequest.tag_ids[0], responseBody?.tags[0].ID);
            Assert.AreEqual(tag.name, responseBody?.tags[0].name);
            Assert.AreEqual(userId, responseBody?.ownerId);
            mockItemRepository.Verify();
        }

        [Test]
        public async Task PatchItems_should_return_200_OK_with_pathced_item()
        {
            var tag = new Tag
            {
                ID = 1,
                name = "new",
                created_at = DateTime.Now
            };
            var existingTag = new Tag
            {
                ID = 1,
                name = "existing",
                created_at = DateTime.Now
            };
            var item = new Item
            {
                content = "**Test**",
                name = "Test",
                ownerId = Guid.NewGuid().ToString("N"),
                tags = new List<Tag> { existingTag },
                guid = Guid.NewGuid().ToString("N"),
                created_at = DateTime.Now
            };
            var itemPatch = new ItemsPatchRequest("patch", new List<int> { 2 });

            var mockItemRepository = new Mock<IItemRepository>();
            mockItemRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(item);
            mockItemRepository.Setup(x => x.Update(It.IsAny<Item>())).Verifiable();
            var mockTagsRepository = new Mock<ITagRepository>();
            mockTagsRepository.Setup(x => x.GetByIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag> { tag });
            var mockUserRepository = new Mock<IUserRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepository.Object);

            var response = await controller.PatchItems(Guid.NewGuid().ToString("N"), itemPatch);
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value as ItemResponse;

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(item.name, responseBody?.name);
            Assert.AreEqual(item.content, responseBody?.content);
            Assert.AreEqual(tag.ID, responseBody?.tags[0].id);
            Assert.AreEqual(tag.name, responseBody?.tags[0].name);
            mockItemRepository.Verify();
        }

        [Test]
        public async Task DeleteItem_should_return_200_OK()
        {
            var mockItemRepositiry = new Mock<IItemRepository>();
            mockItemRepositiry.Setup(x => x.Delete(It.IsAny<string>())).Verifiable();
            var mockTagsRepository = new Mock<ITagRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var controller = new IdbController(mockUserRepository.Object, mockTagsRepository.Object, mockItemRepositiry.Object);

            var response = await controller.DeleteItem(Guid.NewGuid().ToString("N"));
            var okObject = (OkObjectResult)response;
            var statusCode = okObject.StatusCode;
            var responseBody = okObject.Value;

            Assert.AreEqual(200, statusCode);
            Assert.IsNull(responseBody);
            mockItemRepositiry.Verify();
        }

    }
}
