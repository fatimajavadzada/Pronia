using Microsoft.EntityFrameworkCore;

namespace Pronia.Contexts
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}
