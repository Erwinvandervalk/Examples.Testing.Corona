using System;

namespace CoronaTest.Web.Persistence
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset GetNow();
    }
}
