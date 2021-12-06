using idb.Backend.Middlewares;
using idb.Backend.Validators;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace idb.Backend.Tests.Middleware
{
    [TestFixture]
    public class JwtMiddlewareTests
    {
        private readonly Mock<RequestDelegate> mockDelegate;
        private readonly Mock<IJwtTokenValidator> mockJwtTokenValidator;

        public JwtMiddlewareTests()
        {
            mockDelegate = new Mock<RequestDelegate>();
            mockJwtTokenValidator = new Mock<IJwtTokenValidator>();
        }

        [Test]
        public async Task InvokeAsync_should_set_userId_in_HttpContext_Items()
        {
            var requestDelegate = mockDelegate.Object;
            var jwtToken = new JwtSecurityToken(claims: new[] { new Claim("userId", "test") });
            var httpContextMock = new DefaultHttpContext();
            httpContextMock.Request.Headers["Authorization"] = "FancyJwt";

            mockJwtTokenValidator.Setup(x => x.TryValidateJwtToken(It.IsAny<string>(), out jwtToken)).Returns(true);
            var jwtMiddleware = new JwtMiddleware(requestDelegate);

            await jwtMiddleware.InvokeAsync(httpContextMock, mockJwtTokenValidator.Object);
            var itemExists = httpContextMock.Items.TryGetValue("userId", out var expectedValue);

            Assert.AreEqual(true, itemExists);
            Assert.AreEqual("test", expectedValue);
        }

        [Test]
        public async Task InvokeAsync_should_not_set_userId_in_HttpContext_Items()
        {
            var requestDelegate = mockDelegate.Object;
            var jwtToken = new JwtSecurityToken(claims: new[] { new Claim("notUserId", "test") });
            var httpContextMock = new DefaultHttpContext();
            httpContextMock.Request.Headers["Authorization"] = "FancyJwt";

            mockJwtTokenValidator.Setup(x => x.TryValidateJwtToken(It.IsAny<string>(), out jwtToken)).Returns(true);
            var jwtMiddleware = new JwtMiddleware(requestDelegate);

            await jwtMiddleware.InvokeAsync(httpContextMock, mockJwtTokenValidator.Object);
            var itemExists = httpContextMock.Items.TryGetValue("userId", out var expectedValue);

            Assert.AreEqual(true, itemExists);
            Assert.IsNull(expectedValue);
        }
    }
}
