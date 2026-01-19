namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models.Temple;
    using XeniaRegistrationBackend.Models.Token;

    public class SubscribePlanRepository : ISubscribePlanRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly TokenDbContext _tocontext;

        public SubscribePlanRepository(TempleDbContext tecontext, TokenDbContext tocontext)
        {
            _tecontext = tecontext;
            _tocontext = tocontext;
        }


        #region TEMPLE
        public async Task<int> CreateTempleSubscribePlanAsync(SubscribePlanRequestDto request)
        {
            var plan = new TK_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanPrice = request.PlanPrice,
                PlanDurationDays = request.PlanDurationDays,
                PlanUsers = request.PlanUsers,
                PlanIsAddOn = request.planIsAddOn,
                PlanCreatedBy = 0,
                PlanModifiedBy = 0,
                PlanModifiedOn = DateTime.Now,
                PlanActive = request.PlanActive
            };

            _tecontext.SubscribePlan.Add(plan);
            await _tecontext.SaveChangesAsync();

            return plan.PlanId;
        }

        public async Task<bool> CreateTempleSubscribeUpdateAsync(int planId, SubscribePlanRequestDto request)
        {
            var plan = await _tecontext.SubscribePlan.FindAsync(planId);
            if (plan == null) return false;
        
            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanPrice = request.PlanPrice;
            plan.PlanDurationDays = request.PlanDurationDays;
            plan.PlanUsers = request.PlanUsers;
            plan.PlanIsAddOn = request.planIsAddOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedBy = 0;
            plan.PlanModifiedOn = DateTime.Now;

            await _tecontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscribePlanResponseDto>> GetAllTempleSubscriptionPlanAsync()
        {
            return await _tecontext.SubscribePlan
                .Select(p => new SubscribePlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanUsers = p.PlanUsers,
                    PlanActive = p.PlanActive
                })
                .ToListAsync();
        }

        public async Task<SubscribePlanResponseDto?> GetSubscriptionTemplePlanByIdAsync(int planId)
        {
            return await _tecontext.SubscribePlan
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribePlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanActive = p.PlanActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTempleSubscriptionAsync(CompanySubscriptionCreateDto dto)
        {
            using var transaction = await _tecontext.Database.BeginTransactionAsync();

            try
            {
                var plan = await _tecontext.SubscribePlan
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
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

                _tecontext.CompanySubscriptions.Add(subscription);
                await _tecontext.SaveChangesAsync();

                if (dto.Addons != null && dto.Addons.Any())
                {
                    var addons = dto.Addons.Select(a => new TK_CompanySubscriptionAddon
                    {
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        UserCount = a.UserCount
                    });

                    _tecontext.CompanySubscriptionAddon.AddRange(addons);
                    await _tecontext.SaveChangesAsync();
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

        public async Task<int> CreateTempleAddonAsync(CompanySubscriptionAddonCreateDto dto)
        {
            var addon = new TK_CompanySubscriptionAddon
            {
                PlanId = dto.PlanId,
                CompanyId = dto.CompanyId,
                Amount = dto.Amount,
                UserCount = dto.UserCount
            };

            _tecontext.CompanySubscriptionAddon.Add(addon);
            await _tecontext.SaveChangesAsync();

            return addon.SubAddonId;
        }


        #endregion

        #region TOKEN

        public async Task<IEnumerable<xtm_SubscribePlan>> GetAllTokenSubscriptionPlanAsync()
        {
            return await _tocontext.SubscribePlans.ToListAsync();
        }

        public async Task<xtm_SubscribePlan?> GetSubscriptionTokenPlanByIdAsync(int planId)
        {
            return await _tocontext.SubscribePlans.FindAsync(planId);
        }

        public async Task<xtm_SubscribePlan> CreateTokenSubscribePlanAsync(xtm_SubscribePlan plan)
        {
            plan.PlanCreatedOn = DateTime.UtcNow;
            _tocontext.SubscribePlans.Add(plan);
            await _tocontext.SaveChangesAsync();
            return plan;
        }

        public async Task CreateTokenSubscribeUpdateAsync(xtm_SubscribePlan plan)
        {
            plan.PlanModifiedOn = DateTime.UtcNow;
            _tocontext.SubscribePlans.Update(plan);
            await _tocontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int planId)
        {
            var plan = await _tocontext.SubscribePlans.FindAsync(planId);
            if (plan != null)
            {
                _tocontext.SubscribePlans.Remove(plan);
                await _tocontext.SaveChangesAsync();
            }
        }

        #endregion

    }

}
