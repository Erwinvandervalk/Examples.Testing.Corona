using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Queries
{
    public class CoronaTestQueryHandler : IRequestHandler<GetCoronaTestQuery, GetCoronaTestQuery.Response>
    {
        private readonly CoronaTestRepository _repository;

        public CoronaTestQueryHandler(CoronaTestRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetCoronaTestQuery.Response> Handle(GetCoronaTestQuery request,
            CancellationToken cancellationToken)
        {
            var item = await _repository.Get(request.Id, cancellationToken);

            if (item == null)
                return null;
            return new GetCoronaTestQuery.Response
            {
                Id = item.Id,
                Location = item.Location,
                CreatedOn = item.CreatedOn,
                LastChanged = item.LastChanged,
                ScheduledOn = item.ScheduledOn,
                TestSubjectIdentificatieNummer = item.TestSubjectIdentificatieNummer,
                TestSubjectName = item.TestSubjectName,
                AdministeredOn = item.AdministeredOn,
                Status = item.Status,
                ResultsKnown = item.ResultsKnown,
                TestResult = item.TestResult,
                Version = item.Version
            };
        }
    }
}