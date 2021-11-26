using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoronaTest.MockLess.Tests.testinfra;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace CoronaTest.MockLess.Tests
{
    public class ApiTests : IAsyncLifetime
    {
        private HttpClient Client;
        private CoronaTestClient CoronaTestClient;
        private readonly ApiTestFixture Fixture;

        public StaticTestData The = new StaticTestData();

        public TestDataBuilder Some => new TestDataBuilder(The);

        public ApiTests(ITestOutputHelper output)
        {
            Fixture = new ApiTestFixture(output, The);
        }


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
        public async Task Can_get_home()
        {
            var result = await Client.GetAsync("/CoronaTest/" + Guid.NewGuid());
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
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
        public async Task Cannot_schedule_test_in_the_past()
        {
            var result = await CoronaTestClient.ScheduleTest(
                request: Some.ScheduleTestRequest()
                    .With(x => x.ScheduledOn -= TimeSpan.FromHours(1)), 

                expectedStatusCode: HttpStatusCode.BadRequest);
        }
    }
}