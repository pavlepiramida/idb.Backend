using System;

namespace idb.Backend.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public interface IDateTimeProvider
    {
        DateTime UtcNow { get;}
    }
}
