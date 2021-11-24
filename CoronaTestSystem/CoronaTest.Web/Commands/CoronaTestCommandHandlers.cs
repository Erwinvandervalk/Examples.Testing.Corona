using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class ScheduleTestAppointmentCommandHandler : 
        IRequestHandler<ScheduleTestAppointmentCommand, CommandResponse>
    {
        private readonly CoronaTestRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ScheduleTestAppointmentCommandHandler(CoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CommandResponse> Handle(ScheduleTestAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = CoronaTestEntity.ScheduleNewAppointment(
                request.Location, 
                request.ScheduledOn,
                request.TestSubjectIdentificatieNummer, 
                request.TestSubjectName,
                _dateTimeProvider.GetNow());

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
        
    }
    public class RecordTestAdministeredCommandHandler :
      IRequestHandler<RecordTestAdministeredCommand, CommandResponse>
    {
        private readonly CoronaTestRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RecordTestAdministeredCommandHandler(CoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }
        

        public async Task<CommandResponse> Handle(RecordTestAdministeredCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.Get(request.Id, cancellationToken);

            if (appointment == null)
                return new NotFoundResponse();

            if (appointment.Status != TestStatus.Scheduled && appointment.Status != TestStatus.Administered)
            {
                return new FailureResponse()
                {
                    Reason =
                    $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.AdministeredOn = request.AdministeredOn;
            appointment.Status = TestStatus.Administered;
            appointment.Version++;

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
        
    }
    public class RecordTestResultCommandHandler :
      IRequestHandler<RecordTestResultCommand, CommandResponse>
    {
        private readonly CoronaTestRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

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
                return new FailureResponse()
                {
                    Reason =
                        $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.Status = TestStatus.ResultKnown;
            appointment.TestResult = request.TestResult;
            appointment.ResultsKnown = _dateTimeProvider.GetNow();
            appointment.Version++;

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}