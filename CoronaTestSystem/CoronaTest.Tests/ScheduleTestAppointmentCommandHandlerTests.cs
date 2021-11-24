using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Commands;
using CoronaTest.Web.Persistence;
using FakeItEasy;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Shouldly;
using Xunit;

namespace CoronaTest.Tests
{
    public class ScheduleTestAppointmentCommandHandlerTests
    {
        [Fact]
        public void Can_Create()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            // Act & assert
            new ScheduleTestAppointmentCommandHandler(repository, dateTimeProvider);
        }

        [Fact]
        public async Task When_calling_with_null_command_throws()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            // Act
            var sut = new ScheduleTestAppointmentCommandHandler(repository, dateTimeProvider);

            // Assert
            await Should.ThrowAsync<ArgumentNullException>(() => sut.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task When_saving_new_appointment_then_save_is_invoked()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            var now = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.FromHours(6));
            A.CallTo(() => dateTimeProvider.GetNow()).Returns(now);

            var sut = new ScheduleTestAppointmentCommandHandler(repository, dateTimeProvider);

            var scheduleTestAppointmentCommand = new ScheduleTestAppointmentCommand()
            {
                Location = "testlocation",
                ScheduledOn = now,
                TestSubjectIdentificatieNummer = "number",
                TestSubjectName = "name"
            };

            // Act
            await sut.Handle(scheduleTestAppointmentCommand, CancellationToken.None);
;

            // Assert
            A.CallTo(() => repository.SaveAsync(A<CoronaTestEntity>.That.Matches(c =>
                        c.TestSubjectName == scheduleTestAppointmentCommand.TestSubjectName
                        && c.TestSubjectIdentificatieNummer == scheduleTestAppointmentCommand.TestSubjectIdentificatieNummer
                        && c.Location == scheduleTestAppointmentCommand.Location
                        && c.ScheduledOn == now
                        && c.CreatedOn == now
                    )
                    , CancellationToken.None))
                .MustHaveHappened();
        }

        [Fact]
        public async Task When_saving_new_appointment_then_success_is_returned()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            var now = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.FromHours(6));
            A.CallTo(() => dateTimeProvider.GetNow()).Returns(now);

            var sut = new ScheduleTestAppointmentCommandHandler(repository, dateTimeProvider);

            var scheduleTestAppointmentCommand = new ScheduleTestAppointmentCommand()
            {
                Location = "testlocation",
                ScheduledOn = now,
                TestSubjectIdentificatieNummer = "number",
                TestSubjectName = "name"
            };

            // Act
            var result = await sut.Handle(scheduleTestAppointmentCommand, CancellationToken.None);

            // Assert
            var success = result.ShouldBeOfType<SuccessResponse>();
            success.Version.ShouldBe(1);
            success.Id.ShouldNotBe(Guid.Empty);
        }
    }

    
}
