using CoronaTest.MockLess.Web.Controllers;
using CoronaTest.MockLess.Web.Queries;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class TestDataBuilder
    {
        public StaticTestData The;
        public TestDataBuilder(StaticTestData testData)
        {
            The = testData;
        }

        public ScheduleTestRequest ScheduleTestRequest() =>
            new ScheduleTestRequest(location: The.Location,
                scheduledOn: The.CurrentDateTime,
                testSubjectIdentificatieNummer: The.TestSubjectIdentificationNummer,
                testSubjectName: The.TestSubjectName);

        public GetCoronaTestQuery.Response GetScheduledTestResponse()
        {
            return new GetCoronaTestQuery.Response()
            {
                Id = The.TestId,
                CreatedOn = The.CurrentDateTime,
                ScheduledOn = The.CurrentDateTime,
                LastChanged = The.CurrentDateTime,
                Location = The.Location,
                TestSubjectIdentificatieNummer = The.TestSubjectIdentificationNummer,
                TestSubjectName = The.TestSubjectName,
                Version = 1
            };
        }

    }
}