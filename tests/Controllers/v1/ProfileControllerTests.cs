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
    public class ProfileControllerTests
    {
        [Test]
        public async Task Should_return_user()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var expectedUser = new User
            {
                ID = 1,
                guid = "guid",
                email = "pavle",
                first_name = "pavle",
                last_name = "pavle",
                created_at = DateTime.Now,
                is_admin = true
            };
            userRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(expectedUser);
            var controller = new ProfileController(userRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Items.Add("userId", "123");

            var response = await controller.GetProfile();
            var result = (OkObjectResult)response;
            var resultDto = result.Value as UserResponse;

            Assert.IsNotNull(resultDto);
            Assert.AreEqual(expectedUser.ID, resultDto?.id);
            Assert.AreEqual(expectedUser.email, resultDto?.email);
            Assert.AreEqual(expectedUser.guid, resultDto?.guid);
            Assert.AreEqual(expectedUser.is_admin, resultDto?.is_admin);

        }
    }
}
