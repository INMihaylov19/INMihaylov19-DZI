using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Data.Models;

namespace TMS.Data.Data
{
    public class TMSContext : IdentityDbContext<User>
    {
        public TMSContext(DbContextOptions<TMSContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TMS.Data.Models.Task> Tasks { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
