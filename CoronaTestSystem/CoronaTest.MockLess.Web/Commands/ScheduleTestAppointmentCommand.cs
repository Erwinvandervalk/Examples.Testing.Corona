using System;
using MediatR;

namespace CoronaTest.MockLess.Web.Commands
{
    public class ScheduleTestAppointmentCommand : IRequest<CommandResponse>
    {
        public string Location { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public string TestSubjectIdentificatieNummer { get; set; }
        public string TestSubjectName { get; set; }
    }
}