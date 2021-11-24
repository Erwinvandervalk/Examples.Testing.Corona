using Microsoft.EntityFrameworkCore;

namespace CoronaTest.MockLess.Web.Persistence
{
    public class CoronaDbContext : DbContext
    {
        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CoronaTestEntity> CoronaTests { get; set; }
    }
}