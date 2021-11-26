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

        public ScheduleTestAppointmentCommandHandler(CoronaTestRepository repository,
            IDateTimeProvider dateTimeProvider,
            IGuidGenerator guidGenerator,
            ILogger<ScheduleTestAppointmentCommandHandler> logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
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

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }

    public class RecordTestAdministeredCommandHandler :
        IRequestHandler<RecordTestAdministeredCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CoronaTestRepository _repository;

        public RecordTestAdministeredCommandHandler(CoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task<CommandResponse> Handle(RecordTestAdministeredCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _repository.Get(request.Id, cancellationToken);

            if (appointment == null)
                return new NotFoundResponse();

            if (appointment.Status != TestStatus.Scheduled && appointment.Status != TestStatus.Administered)
            {
                return new FailureResponse
                {
                    Reason =
                        $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.AdministeredOn = request.AdministeredOn;
            appointment.Status = TestStatus.Administered;
            appointment.Version++;

            await _repository.SaveAsync(appointment, cancellationToken);

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }

    public class RecordTestResultCommandHandler :
        IRequestHandler<RecordTestResultCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CoronaTestRepository _repository;

        public RecordTestResultCommandHandler(CoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CommandResponse> Handle(RecordTestResultCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.Get(request.Id, cancellationToken);

            if (appointment == null)
                return new NotFoundResponse();

            if (appointment.Status != TestStatus.Administered && appointment.Status != TestStatus.ResultKnown)
            {
                return new FailureResponse
                {
                    Reason =
                        $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.Status = TestStatus.ResultKnown;
            appointment.TestResult = request.TestResult;
            appointment.ResultsKnown = _dateTimeProvider.GetNow();
            appointment.Version++;

            await _repository.SaveAsync(appointment, cancellationToken);

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}