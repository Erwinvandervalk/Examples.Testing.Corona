using System;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class ScheduleTestAppointmentCommandHandler :
        IRequestHandler<ScheduleTestAppointmentCommand, CommandResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICoronaTestRepository _repository;

        public ScheduleTestAppointmentCommandHandler(ICoronaTestRepository repository,
            IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
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

            return new SuccessResponse
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }
}