using idb.Backend.Providers;
using NUnit.Framework;

namespace idb.Backend.Tests.Providers
{
    [TestFixture]
    public class EnvironmentProviderTests
    {
        private EnvironmentProvider _provider;

        public EnvironmentProviderTests()
        {
            _provider = new EnvironmentProvider();
        }

        [Test]
        public void Provider_should_throw_exception_if_variables_not_set()
        {
            Assert.That(() => _provider.JwtLifeTimeMinutes, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.JwtValidateLifeTime, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.JwtKey, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.JwtIssuer, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.JwtAudience, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.Database, Throws.TypeOf<Exception>());
            Assert.That(() => _provider.DatabaseConnection, Throws.TypeOf<Exception>());
        }

        [Test]
        public void Provider_should_get_set_variable()
        {
            var env=SetupEnviormentalVariables();

            Assert.AreEqual(1, _provider.JwtLifeTimeMinutes);
            Assert.IsTrue(_provider.JwtValidateLifeTime);
            Assert.AreEqual(env[nameof(_provider.JwtKey)], _provider.JwtKey);
            Assert.AreEqual(env[nameof(_provider.JwtIssuer)], _provider.JwtIssuer);
            Assert.AreEqual(env[nameof(_provider.JwtAudience)], _provider.JwtAudience);
            Assert.AreEqual(env[nameof(_provider.Database)], _provider.Database);
            Assert.AreEqual(env[nameof(_provider.DatabaseConnection)], _provider.DatabaseConnection);
        }
        [TearDown]
        public void RemoveEnvVariables()
        {
            Environment.SetEnvironmentVariable(nameof(_provider.JwtLifeTimeMinutes),null);
            Environment.SetEnvironmentVariable(nameof(_provider.JwtValidateLifeTime),null);
            Environment.SetEnvironmentVariable(nameof(_provider.JwtKey),null);
            Environment.SetEnvironmentVariable(nameof(_provider.JwtIssuer), null);
            Environment.SetEnvironmentVariable(nameof(_provider.JwtAudience),null);
            Environment.SetEnvironmentVariable(nameof(_provider.Database), null);
            Environment.SetEnvironmentVariable(nameof(_provider.DatabaseConnection), null);
        }
        private Dictionary<string, string> SetupEnviormentalVariables()
        {
            var env = new Dictionary<string, string>();
            Environment.SetEnvironmentVariable(nameof(_provider.JwtLifeTimeMinutes),"1");
            Environment.SetEnvironmentVariable(nameof(_provider.JwtValidateLifeTime), "true");
            Environment.SetEnvironmentVariable(nameof(_provider.JwtKey), nameof(_provider.JwtKey));
            Environment.SetEnvironmentVariable(nameof(_provider.JwtIssuer), nameof(_provider.JwtIssuer));
            Environment.SetEnvironmentVariable(nameof(_provider.JwtAudience), nameof(_provider.JwtAudience));
            Environment.SetEnvironmentVariable(nameof(_provider.Database), nameof(_provider.Database));
            Environment.SetEnvironmentVariable(nameof(_provider.DatabaseConnection), nameof(_provider.DatabaseConnection));

            env.Add(nameof(_provider.JwtLifeTimeMinutes), nameof(_provider.JwtLifeTimeMinutes));
            env.Add(nameof(_provider.JwtValidateLifeTime), nameof(_provider.JwtValidateLifeTime));
            env.Add(nameof(_provider.JwtKey), nameof(_provider.JwtKey));
            env.Add(nameof(_provider.JwtIssuer), nameof(_provider.JwtIssuer));
            env.Add(nameof(_provider.JwtAudience), nameof(_provider.JwtAudience));
            env.Add(nameof(_provider.Database), nameof(_provider.Database));
            env.Add(nameof(_provider.DatabaseConnection), nameof(_provider.DatabaseConnection));
            return env;
        }
    }
}
