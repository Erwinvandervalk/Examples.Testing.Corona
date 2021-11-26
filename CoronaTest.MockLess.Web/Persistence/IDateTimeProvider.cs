using System;

namespace CoronaTest.MockLess.Web.Persistence
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset GetNow();
    }
}