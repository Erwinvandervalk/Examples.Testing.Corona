using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoronaTest.Web.Persistence
{
    public interface ICoronaTestRepository
    {
        Task<CoronaTestEntity> Get(Guid id, CancellationToken ct = default);
        Task SaveAsync(CoronaTestEntity coronaTestEntity, CancellationToken ct);
    }

    public class CoronaTestRepository : ICoronaTestRepository
    {
        private readonly CoronaDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoronaTestRepository(CoronaDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CoronaTestEntity> Get(Guid id, CancellationToken ct = default)
        {
            var item = await _context.CoronaTests.FindAsync(id);
            return item;
        }

        public async Task SaveAsync(CoronaTestEntity coronaTestEntity, CancellationToken ct)
        {
            if (!_context.CoronaTests.Contains(coronaTestEntity))
            {
                _context.CoronaTests.Add(coronaTestEntity);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}