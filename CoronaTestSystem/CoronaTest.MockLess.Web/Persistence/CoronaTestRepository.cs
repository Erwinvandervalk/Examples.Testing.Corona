using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoronaTest.MockLess.Web.Persistence
{
    public class CoronaTestRepository
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

        public async Task SaveAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}