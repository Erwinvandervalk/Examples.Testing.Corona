using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoronaTest.MockLess.Tests.testinfra;
using CoronaTest.MockLess.Web.Persistence;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace CoronaTest.MockLess.Tests
{
    public class ApiTests : IAsyncLifetime
    {
        private readonly ApiTestFixture Fixture;
        private HttpClient Client;
        private CoronaTestClient CoronaTestClient;

        public StaticTestData The = new StaticTestData();

        public ApiTests(ITestOutputHelper output)
        {
            Fixture = new ApiTestFixture(output, The);
        }

        public TestDataBuilder Some => new TestDataBuilder(The);


        public async Task InitializeAsync()
        {
            await Fixture.StartAsync();
            Client = Fixture.BuildClient();
            CoronaTestClient = Fixture.BuildCoronaTestClient();
        }

        public async Task DisposeAsync()
        {
            await Fixture.DisposeAsync();
        }

        [Fact]
        public async Task Can_schedule_appointment()
        {
            await CoronaTestClient.ScheduleTest(
                request: Some.ScheduleTestRequest(),
                expectedStatusCode: HttpStatusCode.OK);

            var (response, result) = await CoronaTestClient.GetTest(The.TestId);

            result.ShouldBeEquivalentTo(Some.GetScheduledTestResponse());
        }

        [Fact]
        public async Task Can_administer_scheduled_test()
        {
            await CoronaTestClient.ScheduleTest(
                request: Some.ScheduleTestRequest(),
                expectedStatusCode: HttpStatusCode.OK);

            await CoronaTestClient.AdministerTest(The.TestId, The.CurrentDateTime.AddDays(1));

            var (response, result) = await CoronaTestClient.GetTest(The.TestId);

            result.ShouldBeEquivalentTo(Some.GetScheduledTestResponse()
                .With(x =>
                    {
                        x.AdministeredOn = The.CurrentDateTime.AddDays(1);
                        x.Status = TestStatus.Administered;
                        x.Version = 2;
                    }));
        }

        [Fact]
        public async Task Cannot_schedule_test_in_the_past()
        {
            var result = await CoronaTestClient.ScheduleTest(
                request: Some.ScheduleTestRequest()
                    .With(x =>
                        {
                            x.ScheduledOn = The.CurrentDateTime.Subtract(TimeSpan.FromHours(1));
                        }),
                expectedStatusCode: HttpStatusCode.BadRequest);
        }
    }
}