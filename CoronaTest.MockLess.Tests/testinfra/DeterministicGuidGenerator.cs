using System;
using CoronaTest.MockLess.Web.Commands;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class DeterministicGuidGenerator : IGuidGenerator
    {
        private readonly StaticTestData _testData;

        private bool _testIdCreated;

        public DeterministicGuidGenerator(StaticTestData testData)
        {
            _testData = testData;
        }

        public Guid GetNext()
        {
            if (!_testIdCreated)
                return _testData.TestId;

            _testIdCreated = true;
            return _testData.NextGuid();
        }
    }
}