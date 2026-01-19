using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Models.Token;

namespace XeniaRegistrationBackend.Models.Temple
{
    public class TokenDbContext : DbContext
    {
        public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options) { }
        public DbSet<xtm_SubscribePlan> SubscribePlans { get; set; }
    }
}
