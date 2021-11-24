using System;

namespace CoronaTest.Web.Persistence
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset GetNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now;
        }
    }
}
