using Microsoft.EntityFrameworkCore;
using XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Models
{
    public class TempleDbContext : DbContext
    {
        public TempleDbContext(DbContextOptions<TempleDbContext> options) : base(options) { }
        public DbSet<TK_Company> Company { get; set; }
        public DbSet<TK_CompanyLabel> CompanyLabel { get; set; }
        public DbSet<TK_CompanySettings> CompanySetting { get; set; }
        public DbSet<TK_CompanySubscription> CompanySubscriptions { get; set; }
        public DbSet<TK_CompanySubscriptionAddon> CompanySubscriptionAddon { get; set; }
        public DbSet<TK_Module> Modules { get; set; }
        public DbSet<TK_SubscribePlan> SubscribePlan { get; set; }
        public DbSet<TK_PlanModuleMap> PlanModuleMap { get; set; }
    }
}
