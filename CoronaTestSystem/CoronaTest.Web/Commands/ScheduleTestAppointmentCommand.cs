using System;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class ScheduleTestAppointmentCommand : IRequest<CommandResponse>
    {
        public string Location { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public string TestSubjectIdentificatieNummer { get; set; }
        public string TestSubjectName { get; set; }



    }

    public abstract class CommandResponse
    {

    }


    public class SuccessResponse : CommandResponse
    {
        public Guid Id { get; set; }
        public long Version { get; set; }
    }

    public class FailureResponse: CommandResponse
    {
        public string Reason { get; set; }
    }

    public class NotFoundResponse : CommandResponse
    {

    }

}