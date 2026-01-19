
using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
  
    }

}

