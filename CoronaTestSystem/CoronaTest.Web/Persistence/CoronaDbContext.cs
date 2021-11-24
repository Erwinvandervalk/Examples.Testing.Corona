using Microsoft.EntityFrameworkCore;

namespace CoronaTest.Web.Persistence
{
    public class CoronaDbContext : DbContext
    {
        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CoronaTestEntity> CoronaTests { get; set; }
    }
}