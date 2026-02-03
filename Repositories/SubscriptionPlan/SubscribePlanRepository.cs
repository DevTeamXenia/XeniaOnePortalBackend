namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models.Temple;
    using XeniaRegistrationBackend.Models.Token;
    using XeniaRegistrationBackend.Models.Token.XeniaTempleBackend.Models;

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
        public async Task<int> CreateTempleSubscribePlanAsync(SubscribeTemplePlanRequestDto request)
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

        public async Task<bool> CreateTempleSubscribeUpdateAsync(int planId, SubscribeTemplePlanRequestDto request)
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

        public async Task<IEnumerable<SubscribeTemplePlanResponseDto>> GetAllTempleSubscriptionPlanAsync()
        {
            return await _tecontext.SubscribePlan
                .Select(p => new SubscribeTemplePlanResponseDto
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

        public async Task<SubscribeTemplePlanResponseDto?> GetSubscriptionTemplePlanByIdAsync(int planId)
        {
            return await _tecontext.SubscribePlan
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribeTemplePlanResponseDto
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

        public async Task<int> CreateTempleSubscriptionAsync(CompanyTempleSubscriptionCreateDto dto)
        {
            using var transaction = await _tecontext.Database.BeginTransactionAsync();

            try
            {

                var activeSubscription = await _tecontext.CompanySubscriptions
               .FirstOrDefaultAsync(s =>
                   s.CompanyId == dto.CompanyId &&
                   s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED"; 
                    activeSubscription.SubscriptionEndDate = DateTime.Now;
                    _tecontext.CompanySubscriptions.Update(activeSubscription);

                 
                    var activeAddons = await _tecontext.CompanySubscriptionAddon
                        .Where(a =>
                            a.CompanyId == dto.CompanyId &&
                            a.Status == "ACTIVE")
                        .ToListAsync();

                    foreach (var addon in activeAddons)
                    {
                        addon.Status = "EXPIRED"; 
                    }

                    _tecontext.CompanySubscriptionAddon.UpdateRange(activeAddons);
                    await _tecontext.SaveChangesAsync();
                }


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
                        MainPlanId = a.MainPlanId,
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        UserCount = a.UserCount,
                        Status = a.Status,
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

        public async Task<int> CreateTempleAddonAsync(CompanyTempleSubscriptionAddonCreateDto dto)
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

        public async Task<int> CreateTokenSubscribePlanAsync(SubscribeTokenPlanRequestDto request)
        {
            var plan = new xtm_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanPrice = request.PlanPrice,
                PlanDurationDays = request.PlanDurationDays,
                PlanDep = request.PlanDeps,
                PlanIsAddOn = request.planIsAddOn,
                PlanCreatedBy = 0,
                PlanModifiedBy = 0,
                PlanModifiedOn = DateTime.Now,
                PlanActive = request.PlanActive
            };

            _tocontext.SubscribePlans.Add(plan);
            await _tocontext.SaveChangesAsync();

            return plan.PlanId;
        }

        public async Task<bool> CreateTokenSubscribeUpdateAsync(int planId, SubscribeTokenPlanRequestDto request)
        {
            var plan = await _tocontext.SubscribePlans.FindAsync(planId);
            if (plan == null) return false;

            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanPrice = request.PlanPrice;
            plan.PlanDurationDays = request.PlanDurationDays;
            plan.PlanDep = request.PlanDeps;
            plan.PlanIsAddOn = request.planIsAddOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedBy = 0;
            plan.PlanModifiedOn = DateTime.Now;

            await _tecontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscribeTokenPlanResponseDto>> GetAllTokenSubscriptionPlanAsync()
        {
            return await _tocontext.SubscribePlans
                .Select(p => new SubscribeTokenPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanDeps = p.PlanDep,
                    PlanActive = p.PlanActive
                })
                .ToListAsync();
        }

        public async Task<SubscribeTokenPlanResponseDto?> GetSubscriptionTokenPlanByIdAsync(int planId)
        {
            return await _tocontext.SubscribePlans
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribeTokenPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanPrice = p.PlanPrice,
                    PlanDurationDays = p.PlanDurationDays,
                    PlanDeps = p.PlanDep,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanActive = p.PlanActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTokenSubscriptionAsync(CompanyTokenSubscriptionCreateDto dto)
        {
            using var transaction = await _tocontext.Database.BeginTransactionAsync();

            try
            {

                var activeSubscription = await _tocontext.CompanySubscriptions
               .FirstOrDefaultAsync(s =>
                   s.CompanyId == dto.CompanyId &&
                   s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED";
                    activeSubscription.SubscriptionEndDate = DateTime.Now;
                    _tocontext.CompanySubscriptions.Update(activeSubscription);


                    var activeAddons = await _tocontext.CompanySubscriptionAddon
                        .Where(a =>
                            a.CompanyId == dto.CompanyId &&
                            a.Status == "ACTIVE")
                        .ToListAsync();

                    foreach (var addon in activeAddons)
                    {
                        addon.Status = "EXPIRED";
                    }

                    _tocontext.CompanySubscriptionAddon.UpdateRange(activeAddons);
                    await _tocontext.SaveChangesAsync();
                }


                var plan = await _tocontext.SubscribePlans
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.PlanActive);

                if (plan == null)
                    throw new Exception("Invalid or inactive plan");

                var startDate = DateTime.Now;
                var endDate = startDate.AddDays(plan.PlanDurationDays).AddTicks(-1);

                var subscription = new xtm_CompanySubscription
                {
                    PlanId = plan.PlanId,
                    CompanyId = dto.CompanyId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = plan.PlanDurationDays,
                    SubscriptionAmount = plan.PlanPrice,
                    SubscriptionDepCount = plan.PlanDep,
                    Status = "ACTIVE"
                };

                _tocontext.CompanySubscriptions.Add(subscription);
                await _tocontext.SaveChangesAsync();

                if (dto.Addons != null && dto.Addons.Any())
                {
                    var addons = dto.Addons.Select(a => new xtm_CompanySubscriptionAddon
                    {
                        MainPlanId = a.MainPlanId,
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        DepCount = a.UserCount,
                        Status = a.Status,
                    });

                    _tocontext.CompanySubscriptionAddon.AddRange(addons);
                    await _tocontext.SaveChangesAsync();
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

        public async Task<int> CreateTokenAddonAsync(CompanyTokenSubscriptionAddonCreateDto dto)
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

    }

}
