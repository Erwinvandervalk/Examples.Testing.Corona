using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoronaTest.Web.Persistence
{
    public interface ICoronaDbContext
    {
        DbSet<CoronaTestEntity> CoronaTests { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        void Dispose();
    }

    public class CoronaDbContext : DbContext, ICoronaDbContext
    {
        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CoronaTestEntity> CoronaTests { get; set; }
    }
}