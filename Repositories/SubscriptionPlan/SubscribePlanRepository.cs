namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models;
    using XeniaTempleBackend.Models;

    public class SubscribePlanRepository : ISubscribePlanRepository
    {
        private readonly TempleDbContext _context;

        public SubscribePlanRepository(TempleDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateSubscribePlanAsync(SubscribePlanRequestDto request)
        {
            var plan = new TK_SubscribePlan
            {
                CompanyId = request.CompanyId,
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanPrice = request.PlanPrice,
                PlanDurationDays = request.PlanDurationDays,
                PlanUsers = request.PlanUsers,
                PlanIsAddeOn = request.planIsAddeOn,
                PlanCreatedBy = 0,
                PlanActive = request.PlanActive
            };

            _context.SubscribePlan.Add(plan);
            await _context.SaveChangesAsync();

            return plan.PlanId;
        }

        public async Task<bool> CreateSubscribeUpdateAsync(int planId, SubscribePlanRequestDto request)
        {
            var plan = await _context.SubscribePlan.FindAsync(planId);
            if (plan == null) return false;

            plan.CompanyId = request.CompanyId;
            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanPrice = request.PlanPrice;
            plan.PlanDurationDays = request.PlanDurationDays;
            plan.PlanUsers = request.PlanUsers;
            plan.PlanIsAddeOn = request.planIsAddeOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedBy = 0;
            plan.PlanModifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscribePlanResponseDto>> GetAllSubscriptionPlanAsync()
        {
            return await _context.SubscribePlan
                .Select(p => new SubscribePlanResponseDto
                {
                    PlanId = p.PlanId,
                    CompanyId = p.CompanyId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanUsers = p.PlanUsers,
                    PlanActive = p.PlanActive
                })
                .ToListAsync();
        }

        public async Task<SubscribePlanResponseDto?> GetSubscriptionPlanByIdAsync(int planId)
        {
            return await _context.SubscribePlan
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribePlanResponseDto
                {
                    PlanId = p.PlanId,
                    CompanyId = p.CompanyId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddeOn = p.PlanIsAddeOn,
                    PlanActive = p.PlanActive
                })
                .FirstOrDefaultAsync();
        }


        public async Task<int> CreateSubscriptionAsync(CompanySubscriptionCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var plan = await _context.SubscribePlan
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.CompanyId == dto.CompanyId &&
                        p.PlanActive);

                if (plan == null)
                    throw new Exception("Invalid or inactive plan");

                var startDate = DateTime.Now;
                var endDate = startDate.AddDays(plan.PlanDurationDays).AddTicks(-1);

                var subscription = new TK_CompanySubscription
                {
                    PlanId = plan.PlanId,
                    CompanyId = dto.CompanyId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = plan.PlanDurationDays,
                    SubscriptionAmount = plan.PlanPrice,
                    subscriptionUserCount = plan.PlanUsers,
                    Status = "ACTIVE"
                };

                _context.CompanySubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                if (dto.Addons != null && dto.Addons.Any())
                {
                    var addons = dto.Addons.Select(a => new TK_CompanySubscriptionAddon
                    {
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        UserCount = a.UserCount
                    });

                    _context.CompanySubscriptionAddon.AddRange(addons);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return subscription.SubId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CreateAddonAsync(CompanySubscriptionAddonCreateDto dto)
        {
            var addon = new TK_CompanySubscriptionAddon
            {
                PlanId = dto.PlanId,
                CompanyId = dto.CompanyId,
                Amount = dto.Amount,
                UserCount = dto.UserCount
            };

            _context.CompanySubscriptionAddon.Add(addon);
            await _context.SaveChangesAsync();

            return addon.SubAddonId;
        }

    }

}
