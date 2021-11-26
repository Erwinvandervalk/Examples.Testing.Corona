using System;
using System.ComponentModel.DataAnnotations;
using CoronaTest.MockLess.Web.Persistence;
using FluentValidation;

namespace CoronaTest.MockLess.Web.Controllers
{
    public class ScheduleTestRequest
    {
        public ScheduleTestRequest(string location, DateTimeOffset scheduledOn, string testSubjectIdentificatieNummer, string testSubjectName)
        {
            Location = location;
            ScheduledOn = scheduledOn;
            TestSubjectIdentificatieNummer = testSubjectIdentificatieNummer;
            TestSubjectName = testSubjectName;
        }

        [Required] public string Location { get; set; }

        [Required] public DateTimeOffset ScheduledOn { get; set; }

        [Required] public string TestSubjectIdentificatieNummer { get; set; }

        [Required] public string TestSubjectName { get; set; }
    }

    public class ScheduleTestRequestValidator : AbstractValidator<ScheduleTestRequest>
    {
        public ScheduleTestRequestValidator(IDateTimeProvider provider)
        {
            RuleFor(x => x.ScheduledOn)
                .GreaterThanOrEqualTo(provider.GetNow());
        }
    }
}