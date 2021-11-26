using System;
using CoronaTest.MockLess.Web.Persistence;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class ManualDateTimeProvider : IDateTimeProvider
    {
        private readonly StaticTestData _testData;

        public ManualDateTimeProvider(StaticTestData testData)
        {
            _testData = testData;
        }

        public DateTimeOffset GetNow()
        {
            return _testData.CurrentDateTime;
        }
    }
}