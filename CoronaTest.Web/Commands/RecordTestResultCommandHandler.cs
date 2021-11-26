using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
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