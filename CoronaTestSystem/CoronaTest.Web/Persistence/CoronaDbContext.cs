using Microsoft.EntityFrameworkCore;

namespace CoronaTest.Web.Persistence
{
    public class CoronaDbContext : DbContext
    {
        public DbSet<CoronaTestEntity> CoronaTests { get; set; }

        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}