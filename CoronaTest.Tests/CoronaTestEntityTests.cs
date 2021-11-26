using System;
using CoronaTest.Web.Persistence;
using Shouldly;
using Xunit;

namespace CoronaTest.Tests
{
    public class CoronaTestEntityTests
    {
        [Fact]
        public void Can_set_properties()
        {
            // Arrange & act
            var coronaTest = new CoronaTestEntity()
            {
                Status = TestStatus.Administered,
                Id = Guid.NewGuid(),
                Location = "location",
                TestSubjectName = "subject",
                TestSubjectIdentificatieNummer = "nummer"
            };

            coronaTest.SetVersion(3);


            // Assert
            coronaTest.Status.ShouldBe(TestStatus.Administered);
            coronaTest.Id.ShouldNotBe(Guid.Empty);
            coronaTest.Location = "location";
            coronaTest.TestSubjectName = "subject";
            coronaTest.TestSubjectIdentificatieNummer = "nummer";
        }


    }
}