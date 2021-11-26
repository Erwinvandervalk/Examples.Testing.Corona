using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoronaTest.Tests.testinfra;
using CoronaTest.Web.Controllers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace CoronaTest.Tests
{
    public class ApiTests : IAsyncLifetime
    {
        private HttpClient Client;
        private CoronaTestClient CoronaTestClient;
        private readonly ApiTestFixture Fixture;

        public ApiTests(ITestOutputHelper output)
        {
            Fixture = new ApiTestFixture(output);
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
            var result = await CoronaTestClient.ScheduleTest(new ScheduleTestRequest
            {
                Location = "test",
                ScheduledOn = DateTimeOffset.Now.AddDays(1),
                TestSubjectIdentificatieNummer = "1",
                TestSubjectName = "2"
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Cannot_schedule_appointment_in_past()
        {
            var result = await CoronaTestClient.ScheduleTest(new ScheduleTestRequest
            {
                Location = "test",
                ScheduledOn = DateTimeOffset.Now.AddDays(-1),
                TestSubjectIdentificatieNummer = "1",
                TestSubjectName = "2"
            }, HttpStatusCode.BadRequest);

            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}