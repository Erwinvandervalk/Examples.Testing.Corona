using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoronaTest.Web.Controllers;
using CoronaTest.Web.Persistence;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace CoronaTest.Tests
{
    public class ScheduleTestRequestValidatorTests
    {
        [Fact]
        public void Given_valid_Request_Then_Is_Valid()
        {
            DateTimeOffset now = new DateTime(2000, 1, 2, 3, 4, 5);

            var request = new ScheduleTestRequest()
            {
                Location = "location",
                ScheduledOn = now.AddHours(1), // must be in the future
            };
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            A.CallTo(() => dateTimeProvider.GetNow()).Returns(now);
            var validator = new ScheduleTestRequestValidator(dateTimeProvider);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue(string.Join(",", result.Errors.Select(x => x.ErrorMessage)));

        }

        [Fact]
        public void When_scheduled_on_is_in_the_past_then_not_valid()
        {
            DateTimeOffset now = new DateTime(2000, 1, 2, 3, 4, 5);

            var request = new ScheduleTestRequest()
            {
                Location = "location",
                ScheduledOn = now.AddHours(-1), // must be in the future
            };
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            A.CallTo(() => dateTimeProvider.GetNow()).Returns(now);
            var validator = new ScheduleTestRequestValidator(dateTimeProvider);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

        }
    }
}
