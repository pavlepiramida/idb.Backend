using idb.Backend.Controllers.v1;
using idb.Backend.DataAccess.Models;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Requests.v1;
using idb.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace idb.Backend.Tests.Controllers.v1
{
    [TestFixture]
    public class AuthControllerTests
    {
        readonly Mock<IAuthJwtService> authServiceMock;
        readonly Mock<IUserRepository> userRepositoryMock;
        readonly MockRepository mockRepository;
        readonly string expectedJwt;
        readonly string email;
        readonly string password;
        public AuthControllerTests()
        {
            expectedJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2MTg1Yzc4YjEyMWRlMzkyYzJmNDE2OWEiLCJuYmYiOjE2Mzg3MzM3MDcsImV4cCI6MTYzOTMwMDEwNywiaWF0IjoxNjM4NzMzNzA3LCJpc3MiOiJpZGIuQmFja2VuZCIsImF1ZCI6ImlkYiJ9.zp9xwb_GTc - a2KlGjd2GeJrai2lXmTVJbJoaiUbSOcs";
            email = "pavle";
            password = "pavle";
            mockRepository = new MockRepository(MockBehavior.Strict);
            authServiceMock = mockRepository.Create<IAuthJwtService>();
            userRepositoryMock = mockRepository.Create<IUserRepository>();
        }

        [Test]
        public async Task Login_should_return_200()
        {
            var controller = SetupController(expectedJwt, new User { email = email, password = password });

            var response = await controller.LoginAsync(new LoginRequest(Email: email, Password: password));
            var results = (OkObjectResult)response;
            var actualJwt = results.Value as TokenResponse;

            Assert.AreEqual(200, results.StatusCode);
            Assert.AreEqual(expectedJwt, actualJwt?.token);
        }

        [Test]
        public async Task Login_should_return_400()
        {
            var controller = SetupController(expectedJwt, new User());

            var response = await controller.LoginAsync(new LoginRequest(Email: email, Password: password));
            var results = (BadRequestResult)response;

            Assert.AreEqual(400, results.StatusCode);
        }

        [Test]
        public async Task Login_should_return_404()
        {
            var controller = SetupController(expectedJwt, null);

            var response = await controller.LoginAsync(new LoginRequest(Email: email, Password: password));
            var results = (StatusCodeResult)response;

            Assert.AreEqual(404, results.StatusCode);
        }

        private AuthController SetupController(string jwt, User? user)
        {
            authServiceMock.Setup(x => x.GenerateJwt(It.IsAny<string>())).Returns(jwt);
            userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
            return new AuthController(authServiceMock.Object, userRepositoryMock.Object);
        }
    }
}
