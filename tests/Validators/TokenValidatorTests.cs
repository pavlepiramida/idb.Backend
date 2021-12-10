using idb.Backend.Providers;
using idb.Backend.Services;
using idb.Backend.Validators;
using NUnit.Framework;

namespace idb.Backend.Tests.Validators
{
    [TestFixture]
    public class TokenValidatorTests
    {
        private readonly EnvironmentProvider _envProvider;
        private readonly DateTimeProvider _dateTimeProvider;

        public TokenValidatorTests()
        {
            _envProvider = new EnvironmentProvider();
            _dateTimeProvider = new DateTimeProvider();
        }

        [Test]
        public void Validator_should_return_true_if_token_is_valid()
        {
            string userId = "123";
            var jwtToken = GenerateJwt("123");
            var validator = new TokenValidator(_envProvider);

            var isValidToken = validator.TryValidateJwtToken(jwtToken, out var jwtSecurityToken);

            Assert.IsTrue(isValidToken);
            Assert.AreEqual(userId, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
        }

        [Test]
        public void Validator_should_return_false_if_token_is_not_valid()
        {
            var jwtToken = "not.a.token";
            var validator = new TokenValidator(_envProvider);

            var isValidToken = validator.TryValidateJwtToken(jwtToken, out var jwtSecurityToken);

            Assert.IsFalse(isValidToken);
            Assert.IsNull(jwtSecurityToken);
        }

        [SetUp]
        public void SetupEnv()
        {
            Environment.SetEnvironmentVariable("JwtLifeTimeMinutes", "100");
            Environment.SetEnvironmentVariable("JwtValidateLifeTime", "true");
            Environment.SetEnvironmentVariable("JwtKey", "super_long_secerete_key");
            Environment.SetEnvironmentVariable("JwtIssuer", "idb");
            Environment.SetEnvironmentVariable("JwtAudience", "idb");
        }

        public string GenerateJwt(string userId)
        {
            var auth = new AuthService(_envProvider, _dateTimeProvider);
            return auth.GenerateJwt(userId);
        }
    }
}
