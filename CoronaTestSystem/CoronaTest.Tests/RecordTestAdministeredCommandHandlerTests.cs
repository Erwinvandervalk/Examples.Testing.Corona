using System;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Commands;
using CoronaTest.Web.Persistence;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace CoronaTest.Tests
{
    public class RecordTestAdministeredCommandHandlerTests
    {
        [Fact]
        public void Can_Create()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            // Act && assert
            new RecordTestAdministeredCommandHandler(repository, dateTimeProvider);
        }

        [Fact]
        public async Task When_calling_with_null_command_throws()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            // Act
            var sut = new RecordTestAdministeredCommandHandler(repository, dateTimeProvider);

            // Assert
            await Should.ThrowAsync<ArgumentNullException>(() => sut.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async void When_confirming_non_existent_appointment_returns_not_found()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            A.CallTo(() => repository.Get(An<Guid>.Ignored, CancellationToken.None))
                .Returns(Task.FromResult<CoronaTestEntity>(null));

            var sut = new RecordTestAdministeredCommandHandler(repository, dateTimeProvider);

            // Act
            var result = await sut.Handle(new RecordTestAdministeredCommand()
            {
                Id = Guid.NewGuid(),
                AdministeredOn = DateTimeOffset.Now
            }, CancellationToken.None);

            result.ShouldBeOfType<NotFoundResponse>();

        }

        [Fact]
        public async void When_confirming_existing_appointment_then_returns_success_response()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            A.CallTo(() => repository.Get(An<Guid>.Ignored, CancellationToken.None))
                .Returns(Task.FromResult<CoronaTestEntity>(new CoronaTestEntity()
                {
                    Id = Guid.NewGuid(),
                    Location = "a location",
                    Status = TestStatus.Scheduled,
                    TestSubjectIdentificatieNummer = "number",
                    TestSubjectName = "name",
                    Version = 1
                }));

            var sut = new RecordTestAdministeredCommandHandler(repository, dateTimeProvider);

            // Act
            var result = await sut.Handle(new RecordTestAdministeredCommand()
            {
                Id = Guid.NewGuid(),
                AdministeredOn = DateTimeOffset.Now
            }, CancellationToken.None);

            var success = result.ShouldBeOfType<SuccessResponse>();
            success.Version.ShouldBe(2);
            success.Id.ShouldNotBe(Guid.Empty);
        }


        [Fact]
        public async void When_confirming_existing_appointment_then_invokes_save()
        {
            // Arrange
            var repository = A.Fake<ICoronaTestRepository>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            var coronaTestEntity = new CoronaTestEntity()
            {
                Location = "a location",
                Status = TestStatus.Scheduled,
                TestSubjectIdentificatieNummer = "number",
                TestSubjectName = "name"
            };
            A.CallTo(() => repository.Get(An<Guid>.Ignored, CancellationToken.None))
                .Returns(Task.FromResult<CoronaTestEntity>(coronaTestEntity));

            var sut = new RecordTestAdministeredCommandHandler(repository, dateTimeProvider);

            // Act
            await sut.Handle(new RecordTestAdministeredCommand()
            {
                Id = Guid.NewGuid(),
                AdministeredOn = DateTimeOffset.Now
            }, CancellationToken.None);

            A.CallTo(() => repository.SaveAsync(coronaTestEntity, CancellationToken.None)).MustHaveHappened();

        }
    }
}