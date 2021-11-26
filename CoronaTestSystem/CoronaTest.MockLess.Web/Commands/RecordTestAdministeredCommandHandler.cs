using System.Threading;
using System.Threading.Tasks;
using CoronaTest.MockLess.Web.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoronaTest.MockLess.Web.Commands
{
    public class RecordTestAdministeredCommandHandler :
        IRequestHandler<RecordTestAdministeredCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<RecordTestAdministeredCommandHandler> _logger;
        private readonly CoronaTestRepository _repository;

        public RecordTestAdministeredCommandHandler(CoronaTestRepository repository, 
            IDateTimeProvider dateTimeProvider,
            ILogger<RecordTestAdministeredCommandHandler> logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
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

            _logger.LogInformation("Saved {@appointment}", appointment);

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}