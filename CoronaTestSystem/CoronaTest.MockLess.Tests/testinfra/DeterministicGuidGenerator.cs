using System;
using CoronaTest.MockLess.Web.Commands;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class DeterministicGuidGenerator : IGuidGenerator
    {
        private readonly StaticTestData _testData;

        public DeterministicGuidGenerator(StaticTestData testData)
        {
            _testData = testData;
        }

        private bool _testIdCreated = false;

        public Guid GetNext()
        {
            if (!_testIdCreated)
                return _testData.TestId;

            _testIdCreated = true;
            return _testData.NextGuid();
        }
    }
}