using idb.Backend.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace idb.Backend.Tests.Attributes
{
    [TestFixture]
    public class AuthorizeAttributeTests
    {
        [Test]
        public void OnAuthorization_should_set_context_results_to_unauthorized()
        {
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items).Returns(new Dictionary<object, object?>());
            var actionContext = new ActionContext(httpContextMock.Object,
                new RouteData(),
                new ActionDescriptor());
            var authFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata> { });
            var attribute = new AuthorizeAttribute();

            attribute.OnAuthorization(authFilterContext);
            var actualResult = authFilterContext.Result as JsonResult;

            Assert.AreEqual(401, actualResult?.StatusCode);
        }
    }
}
