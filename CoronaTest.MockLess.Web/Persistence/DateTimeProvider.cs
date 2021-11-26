using System;

namespace CoronaTest.MockLess.Web.Persistence
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now;
        }
    }
}