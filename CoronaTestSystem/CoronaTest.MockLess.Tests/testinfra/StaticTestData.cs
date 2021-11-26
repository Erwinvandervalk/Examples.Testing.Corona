using System;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class StaticTestData
    {
        private SequentialGuid _guidSeed;

        public DateTimeOffset CurrentDateTime = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.FromHours(6));

        public string Location = "_a_location_";

        public Guid TestId;
        public string TestSubjectIdentificationNummer = "_an_id_number_";
        public string TestSubjectName = "_an_id_number_";

        public StaticTestData()
        {
            TestId = NextGuid();
        }

        public void TimeTravel(TimeSpan by) => CurrentDateTime += by;

        public Guid NextGuid() => _guidSeed = _guidSeed.GetNext();
    }
}