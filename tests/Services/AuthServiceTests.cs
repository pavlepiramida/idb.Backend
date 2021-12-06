using idb.Backend.Providers;
using idb.Backend.Services;
using Moq;
using NUnit.Framework;

namespace idb.Backend.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private DateTimeProvider _dateTimeProvider;
        private Mock<IJwtEnvironmentProvider> _mockIJwtEnvironmentProvider;

        public AuthServiceTests()
        {
            _dateTimeProvider = new();
            _mockIJwtEnvironmentProvider = new Mock<IJwtEnvironmentProvider>();
        }

        [Test]
        public void GenerateJwt_should_return_jwt_string_token()
        {
            var jwtEnviormentProvider = SetupJwtEnv();

            var service = new AuthService(jwtEnviormentProvider, _dateTimeProvider);
            var actualResults = service.GenerateJwt("1234");

            Assert.AreEqual(false, string.IsNullOrEmpty(actualResults));
            Assert.IsInstanceOf<string>(actualResults);
        }

        private IJwtEnvironmentProvider SetupJwtEnv()
        {
            _mockIJwtEnvironmentProvider.Setup(x => x.JwtAudience).Returns("idb");
            _mockIJwtEnvironmentProvider.Setup(x => x.JwtIssuer).Returns("idb");
            _mockIJwtEnvironmentProvider.Setup(x => x.JwtKey).Returns("longassstringlel123");
            _mockIJwtEnvironmentProvider.Setup(x => x.JwtLifeTimeMinutes).Returns(1440);
            _mockIJwtEnvironmentProvider.Setup(x => x.JwtValidateLifeTime).Returns(true);

            return _mockIJwtEnvironmentProvider.Object;
        }

    }
}
