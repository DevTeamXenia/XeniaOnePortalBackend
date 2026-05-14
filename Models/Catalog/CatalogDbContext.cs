using Microsoft.EntityFrameworkCore;

namespace XeniaRegistrationBackend.Models.Catalog
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<CT_tblCompany> tblCompany { get; set; }
        public DbSet<CT_CompanySubscription> CompanySubscriptions { get; set; }
        public DbSet<CT_CompanySubscriptionAddon> CompanySubscriptionAddon { get; set; }
        public DbSet<CT_SubscribePlan> SubscribePlan { get; set; }
        public DbSet<CT_SubscribePlanDuration> SubscribePlanDuration { get; set; }
        public DbSet<CT_Module> Modules { get; set; }
        public DbSet<CT_PlanModuleMap> PlanModuleMap { get; set; }
        public DbSet<CT_UserSetting> UserSettings { get; set; }
        public DbSet<CT_CompanyLabel> CompanyLabel { get; set; }
        public DbSet<CT_tblCompanySettings> tblCompanySettings { get; set; }
        public DbSet<CT_SubscriptionSpecialRate> SubscriptionSpecialRate { get; set; }

    }
}
