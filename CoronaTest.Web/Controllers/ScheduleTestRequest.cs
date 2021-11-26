using System;
using System.ComponentModel.DataAnnotations;
using CoronaTest.Web.Persistence;
using FluentValidation;

namespace CoronaTest.Web.Controllers
{
    public class ScheduleTestRequest
    {
        [Required, MinLength(1)] 
        public string Location { get; set; }

        [Required] public DateTimeOffset ScheduledOn { get; set; }

        [Required, MinLength(1)] 
        public string TestSubjectIdentificatieNummer { get; set; }

        [Required, MinLength(1)] 
        public string TestSubjectName { get; set; }
    }

    public class ScheduleTestRequestValidator : AbstractValidator<ScheduleTestRequest>
    {
        public ScheduleTestRequestValidator(IDateTimeProvider provider)
        {
            RuleFor(x => x.ScheduledOn)
                .GreaterThan(provider.GetNow());
        }
    }
}