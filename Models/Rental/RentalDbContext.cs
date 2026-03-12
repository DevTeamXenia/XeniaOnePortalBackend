using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Models.Rental
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options) { }
        public DbSet<XRS_Company> Company { get; set; }
        public DbSet<XRS_CompanySettings> CompanySetting { get; set; }
        public DbSet<XRS_CompanySubscription> CompanySubscription { get; set; }
        public DbSet<XRS_SubscribePlan> SubscribePlan { get; set; }
        public DbSet<XRS_SubscribePlanDuration> SubscribePlanDurations { get; set; }
        public DbSet<XRS_Module> Module { get; set; }
        public DbSet<XRS_PlanModuleMap> PlanModuleMap { get; set; }
    }
}
