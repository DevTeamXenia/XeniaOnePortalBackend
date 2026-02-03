using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Models.Token;
using XeniaRegistrationBackend.Models.Token.XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Models.Temple
{
    public class TokenDbContext : DbContext
    {
        public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options) { }

        public DbSet<xtm_Company> Company { get; set; }
        public DbSet<xtm_SubscribePlan> SubscribePlans { get; set; }
        public DbSet<xtm_CompanySubscription> CompanySubscriptions { get; set; }
        public DbSet<xtm_CompanySubscriptionAddon> CompanySubscriptionAddon { get; set; }
        public DbSet<xtm_CompanySettings> CompanySettings { get; set; }

    }
}
