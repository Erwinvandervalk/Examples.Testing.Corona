using System;

namespace CoronaTest.Web.Persistence
{
    public class CoronaTestEntity
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastChanged { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public string TestSubjectIdentificatieNummer { get; set; }
        public string TestSubjectName { get; set; }
        public DateTimeOffset? AdministeredOn { get; set; }

        public TestStatus Status { get; set; }

        public DateTimeOffset? ResultsKnown { get; set; }
        public TestResult TestResult { get; set; }
        public long Version { get; set; }

        public static CoronaTestEntity ScheduleNewAppointment(string location, DateTimeOffset scheduledOn,
            string testSubjectIdentificatieNummer, string testSubjectName, DateTimeOffset now)
        {
            var entity = new CoronaTestEntity
            {
                Id = Guid.NewGuid(),
                ScheduledOn = scheduledOn,
                TestSubjectIdentificatieNummer = testSubjectIdentificatieNummer,
                TestSubjectName = testSubjectName,
                Version = 1,
                CreatedOn = now,
                LastChanged = now
            };
            return entity;
        }
    }
}