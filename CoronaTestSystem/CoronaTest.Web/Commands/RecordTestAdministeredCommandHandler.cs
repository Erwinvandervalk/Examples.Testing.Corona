using System;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class RecordTestAdministeredCommandHandler :
        IRequestHandler<RecordTestAdministeredCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICoronaTestRepository _repository;

        public RecordTestAdministeredCommandHandler(ICoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task<CommandResponse> Handle(RecordTestAdministeredCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

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
}