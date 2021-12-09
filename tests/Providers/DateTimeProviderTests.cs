using idb.Backend.Providers;
using NUnit.Framework;

namespace idb.Backend.Tests.Providers
{
    [TestFixture]
    public class DateTimeProviderTests
    {
        [Test]
        public void DateTimeProvider_should_return_dateTime_not_older_then_5_min()
        {
            double diffMargine = 5;

            var dateTimeProvider = new DateTimeProvider();
            var dateTime = dateTimeProvider.UtcNow;
            var diff = DateTime.UtcNow - dateTime;

            Assert.IsTrue(diffMargine > diff.TotalMinutes);
        }
    }
}
