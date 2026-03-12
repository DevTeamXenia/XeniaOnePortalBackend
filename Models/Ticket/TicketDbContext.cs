using Microsoft.EntityFrameworkCore;

namespace XeniaRegistrationBackend.Models.Ticket
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }
        public DbSet<TI_Company> Company { get; set; }
        public DbSet<TI_CompanyLabel> CompanyLabel { get; set; }
        public DbSet<TI_CompanySettings> CompanySettings { get; set; }
        public DbSet<TI_CompanySubscription> CompanySubscription { get; set; }
        public DbSet<TI_CompanySubscriptionAddon> CompanySubscriptionAddon { get; set; }
        public DbSet<TI_SubscribePlan> SubscribePlan { get; set; }
        public DbSet<TI_SubscribePlanDuration> SubscribePlanDuration { get; set; }
    }
}
