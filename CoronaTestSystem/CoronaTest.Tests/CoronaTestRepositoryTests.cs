using System;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace CoronaTest.Tests
{
    public class CoronaTestRepositoryTests
    {
        [Fact]
        public void Can_Create()
        {
            // Arrange
            var dbContext = A.Fake<ICoronaDbContext>();
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            // Act && assert
            new CoronaTestRepository(dbContext, dateTimeProvider);
        }


        [Fact]
        public async Task Can_save_new_entity()
        {
            // Arrange
            var entity = new CoronaTestEntity()
            {
                Id = Guid.NewGuid(),
                Location = "a location",
                Status = TestStatus.Scheduled,
                TestSubjectIdentificatieNummer = "number",
                TestSubjectName = "name",
                Version = 1
            };

            var options = new DbContextOptionsBuilder<CoronaDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            var dbContext = new CoronaDbContext(options);

            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            var sut = new CoronaTestRepository(dbContext, dateTimeProvider);

            // Act
            await sut.SaveAsync(entity, CancellationToken.None);

            // Assert
            var saved = await sut.Get(entity.Id, CancellationToken.None);
            saved.ShouldBe(entity);

        }
    }
}