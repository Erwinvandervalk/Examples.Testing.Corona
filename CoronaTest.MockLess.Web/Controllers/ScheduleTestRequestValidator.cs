using CoronaTest.MockLess.Web.Persistence;
using FluentValidation;

namespace CoronaTest.MockLess.Web.Controllers
{
    public class ScheduleTestRequestValidator : AbstractValidator<ScheduleTestRequest>
    {
        public ScheduleTestRequestValidator(IDateTimeProvider provider)
        {
            RuleFor(x => x.ScheduledOn)
                .GreaterThan(provider.GetNow());
        }
    }
}