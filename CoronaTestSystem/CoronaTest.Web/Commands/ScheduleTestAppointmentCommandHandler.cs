using System;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Integration;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class ScheduleTestAppointmentCommandHandler :
        IRequestHandler<ScheduleTestAppointmentCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICoronaTestRepository _repository;
        //private readonly IEmailSender _emailSender;

        public ScheduleTestAppointmentCommandHandler(ICoronaTestRepository repository,
          //  IEmailSender emailSender,
            IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            //_emailSender = emailSender;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CommandResponse> Handle(ScheduleTestAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var appointment = CoronaTestEntity.ScheduleNewAppointment(
                request.Location,
                request.ScheduledOn,
                request.TestSubjectIdentificatieNummer,
                request.TestSubjectName,
                _dateTimeProvider.GetNow());

            await _repository.SaveAsync(appointment, cancellationToken);

            // Yes, the following call is not really idempotent. 
            // Idempotency is not the thing i'm focusing on here. 

//            _emailSender.SendReminderEmail(request.TestSubjectName, "Your corona test", $"Please show up at your corona test on {request.ScheduledOn.Date} at {request.Location}");

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}