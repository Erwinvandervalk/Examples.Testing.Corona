using System.Threading;
using System.Threading.Tasks;
using CoronaTest.MockLess.Web.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoronaTest.MockLess.Web.Commands
{
    public class ScheduleTestAppointmentCommandHandler :
        IRequestHandler<ScheduleTestAppointmentCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<ScheduleTestAppointmentCommandHandler> _logger;
        private readonly CoronaTestRepository _repository;
        private readonly IEmailSender _emailSender;

        public ScheduleTestAppointmentCommandHandler(CoronaTestRepository repository,
            IDateTimeProvider dateTimeProvider,
            IEmailSender emailSender,
            IGuidGenerator guidGenerator,
            ILogger<ScheduleTestAppointmentCommandHandler> logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _emailSender = emailSender;
            _guidGenerator = guidGenerator;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(ScheduleTestAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = CoronaTestEntity.ScheduleNewAppointment(
                _guidGenerator.GetNext(),
                request.Location,
                request.ScheduledOn,
                request.TestSubjectIdentificatieNummer,
                request.TestSubjectName,
                _dateTimeProvider.GetNow());

            await _repository.SaveAsync(appointment, cancellationToken);

            _logger.LogInformation("Saved {@testAppointment}", appointment);

            // Yes, the following call is not really idempotent. 
            // Idempotency is not the thing i'm focusing on here. 

            _emailSender.SendReminderEmail(request.TestSubjectName, "Your corona test", $"Please show up at your corona test on {request.ScheduledOn.Date} at {request.Location}");


            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}