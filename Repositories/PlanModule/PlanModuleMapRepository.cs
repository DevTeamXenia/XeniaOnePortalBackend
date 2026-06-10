namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models;
    using XeniaRegistrationBackend.Models.Catalog;
    using XeniaRegistrationBackend.Models.Rental;
    using XeniaRegistrationBackend.Models.Temple;

    public class PlanModuleMapRepository : IPlanModuleMapRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly RentalDbContext _recontext;
        private readonly CatalogDbContext _cacontext;

        public PlanModuleMapRepository(TempleDbContext tecontext, RentalDbContext recontext, CatalogDbContext cacontext)
        {
            _tecontext = tecontext;
            _recontext = recontext;
            _cacontext = cacontext;


        }

        public async Task<List<int>> CreateTemplePlanModuleAsync(List<TK_PlanModuleMap> request)
        {
            var ids = new List<int>();

            if (request == null || !request.Any())
                return ids;

            var planIds = request.Select(x => x.PlanId).Distinct().ToList();

            if (planIds.Count == 1)
            {
                var planId = planIds.First();
                var requestedModuleIds = request.Select(x => x.ModuleId).OrderBy(id => id).ToList();


                var allExistingPlans = await _tecontext.PlanModuleMap
                    .GroupBy(x => x.PlanId)
                    .Select(g => new
                    {
                        PlanId = g.Key,
                        ModuleIds = g.Select(x => x.ModuleId).OrderBy(id => id).ToList()
                    })
                    .ToListAsync();

                var duplicatePlan = allExistingPlans
                    .FirstOrDefault(p => p.PlanId != planId &&
                                        p.ModuleIds.SequenceEqual(requestedModuleIds));

                if (duplicatePlan != null)
                {
                    throw new InvalidOperationException(
                        $"A plan with ID {duplicatePlan.PlanId} already has the exact same modules. " +
                        "Cannot create duplicate plan configuration.");
                }

                var hasActiveSubscriptions = await _tecontext.CompanySubscriptions
                    .AnyAsync(cs => cs.PlanId == planId);

                if (hasActiveSubscriptions)
                {
                    throw new InvalidOperationException(
                        $"Cannot update Plan ID {planId} because it has active company subscriptions. " +
                        "Please deactivate all subscriptions before updating the plan modules.");
                }
            }

            var existingRecords = await _tecontext.PlanModuleMap
                .Where(x => planIds.Contains(x.PlanId))
                .ToListAsync();

            if (existingRecords.Any())
            {
                _tecontext.PlanModuleMap.RemoveRange(existingRecords);
                await _tecontext.SaveChangesAsync();
            }

            var newRecords = request.Select(x => new TK_PlanModuleMap
            {
                PlanId = x.PlanId,
                ModuleId = x.ModuleId,
                Active = x.Active
            }).ToList();

            await _tecontext.PlanModuleMap.AddRangeAsync(newRecords);
            await _tecontext.SaveChangesAsync();

            ids = newRecords.Select(x => x.SubPlanId).ToList();

            return ids;
        }

        public async Task<PlanModuleGroupResponseDto?> GetTemplePlanModuleByIdAsync(int subPlanId)
        {
            var query = await (
                from pm in _tecontext.PlanModuleMap
                join p in _tecontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _tecontext.Modules on pm.ModuleId equals m.ModuleId
                select new
                {
                    p.PlanId,
                    p.PlanName,
                    pm.SubPlanId,
                    m.ModuleId,
                    m.ModuleName,
                    pm.Active
                }
            ).ToListAsync();

            var planWithModule = query.FirstOrDefault(x => x.SubPlanId == subPlanId);

            if (planWithModule == null)
                return null;

            // Get all modules for that plan
            var result = query
                .Where(x => x.PlanId == planWithModule.PlanId)
                .GroupBy(x => new { x.PlanId, x.PlanName })
                .Select(g => new PlanModuleGroupResponseDto
                {
                    PlanId = g.Key.PlanId,
                    PlanName = g.Key.PlanName,
                    Modules = g.Select(m => new ModuleItemDto
                    {
                        SubPlanId = m.SubPlanId,
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        Active = m.Active
                    }).ToList()
                })
                .FirstOrDefault();

            return result;
        }

        public async Task<List<PlanModuleGroupResponseDto>> GetTempleAllAsync()
        {
            var query = await (
                from pm in _tecontext.PlanModuleMap
                join p in _tecontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _tecontext.Modules on pm.ModuleId equals m.ModuleId
                select new
                {
                    p.PlanId,
                    p.PlanName,
                    pm.SubPlanId,
                    m.ModuleId,
                    m.ModuleName,
                    pm.Active
                }
            ).ToListAsync();

            var result = query
                .GroupBy(x => new { x.PlanId, x.PlanName })
                .Select(g => new PlanModuleGroupResponseDto
                {
                    PlanId = g.Key.PlanId,
                    PlanName = g.Key.PlanName,
                    Modules = g.Select(m => new ModuleItemDto
                    {
                        SubPlanId = m.SubPlanId,
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        Active = m.Active
                    }).ToList()
                })
                .ToList();

            return result;
        }


        #region CATALOG 

        public async Task<List<int>> CreateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request)
        {
            var maps = request.Select(x => new CT_PlanModuleMap
            {
                PlanId = x.PlanId,
                ModuleId = x.ModuleId,
                Active = x.Active
            }).ToList();

            await _cacontext.PlanModuleMap.AddRangeAsync(maps);
            await _cacontext.SaveChangesAsync();

            return maps.Select(x => x.SubPlanId).ToList();
        }


        public async Task<bool> UpdateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request)
        {
            if (request == null || !request.Any())
                return false;

            var ids = request.Select(x => x.SubPlanId).ToList();

            var existing = await _cacontext.PlanModuleMap
                .Where(x => ids.Contains(x.SubPlanId))
                .ToListAsync();

            foreach (var map in existing)
            {
                var updated = request.FirstOrDefault(x => x.SubPlanId == map.SubPlanId);
                if (updated == null) continue;

                map.PlanId = updated.PlanId;
                map.ModuleId = updated.ModuleId;
                map.Active = updated.Active;
            }

            await _cacontext.SaveChangesAsync();
            return true;
        }


        public async Task<SubscriptionCatalogPlanResponseDto?> GetCatalogPlanModuleByIdAsync(int id)
        {
            return await (
                from pm in _cacontext.PlanModuleMap
                join p in _cacontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _cacontext.Modules on pm.ModuleId equals m.ModuleId
                where pm.SubPlanId == id
                select new SubscriptionCatalogPlanResponseDto
                {
                    SubPlanId = pm.SubPlanId,
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    Active = pm.Active
                }
            ).FirstOrDefaultAsync();
        }


        public async Task<List<SubscriptionCatalogPlanResponseDto>> GetCatalogAllAsync()
        {
            return await (
                from pm in _cacontext.PlanModuleMap
                join p in _cacontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _cacontext.Modules on pm.ModuleId equals m.ModuleId
                select new SubscriptionCatalogPlanResponseDto
                {
                    SubPlanId = pm.SubPlanId,
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    Active = pm.Active
                }
            ).ToListAsync();
        }

        #endregion

        #region Rental
        public async Task<List<int>> CreateRentalPlanModuleAsync(List<XRS_PlanModuleMap> request)
        {
            var ids = new List<int>();

            if (request == null || !request.Any())
                return ids;

            var planIds = request.Select(x => x.PlanId).Distinct().ToList();

            // REMOVE existing mappings for these plans
            var existing = await _recontext.PlanModuleMap
                .Where(x => planIds.Contains(x.PlanId))
                .ToListAsync();

            if (existing.Any())
            {
                _recontext.PlanModuleMap.RemoveRange(existing);
                await _recontext.SaveChangesAsync();
            }

            // REINSERT new mappings
            var newRecords = request.Select(x => new XRS_PlanModuleMap
            {
                PlanId = x.PlanId,
                ModuleId = x.ModuleId,
                Active = x.Active
            }).ToList();

            await _recontext.PlanModuleMap.AddRangeAsync(newRecords);
            await _recontext.SaveChangesAsync();

            ids = newRecords.Select(x => x.SubPlanId).ToList();

            return ids;
        }


        public async Task<PlanModuleGroupResponseDto?> GetRentalPlanModuleByIdAsync(int planId)
        {
            var query = await (
                from pm in _recontext.PlanModuleMap
                join p in _recontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _recontext.Module on pm.ModuleId equals m.ModuleId
                where pm.PlanId == planId
                select new
                {
                    p.PlanId,
                    p.PlanName,
                    pm.SubPlanId,
                    m.ModuleId,
                    m.ModuleName,
                    pm.Active
                }
            ).ToListAsync();

            if (!query.Any())
                return null;

            var result = query
                .GroupBy(x => new { x.PlanId, x.PlanName })
                .Select(g => new PlanModuleGroupResponseDto
                {
                    PlanId = g.Key.PlanId,
                    PlanName = g.Key.PlanName,
                    Modules = g.Select(m => new ModuleItemDto
                    {
                        SubPlanId = m.SubPlanId,
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        Active = m.Active
                    }).ToList()
                })
                .FirstOrDefault();

            return result;
        }

        public async Task<List<PlanModuleGroupResponseDto>> GetRentalAllAsync()
        {
            var query = await (
                from pm in _recontext.PlanModuleMap
                join p in _recontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _recontext.Module on pm.ModuleId equals m.ModuleId
                select new
                {
                    p.PlanId,
                    p.PlanName,
                    pm.SubPlanId,
                    m.ModuleId,
                    m.ModuleName,
                    pm.Active
                }
            ).ToListAsync();

            var result = query
                .GroupBy(x => new { x.PlanId, x.PlanName })
                .Select(g => new PlanModuleGroupResponseDto
                {
                    PlanId = g.Key.PlanId,
                    PlanName = g.Key.PlanName,
                    Modules = g.Select(m => new ModuleItemDto
                    {
                        SubPlanId = m.SubPlanId,
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        Active = m.Active
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }
}
#endregion



