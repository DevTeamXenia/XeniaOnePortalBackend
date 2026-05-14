namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
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
            var maps = request.Select(x => new TK_PlanModuleMap
            {
                PlanId = x.PlanId,
                ModuleId = x.ModuleId,
                Active = x.Active
            }).ToList();

            await _tecontext.PlanModuleMap.AddRangeAsync(maps);
            await _tecontext.SaveChangesAsync();

            return maps.Select(x => x.SubPlanId).ToList();
        }

        public async Task<bool> UpdateTemplePlanModuleAsync(List<TK_PlanModuleMap> request)
        {
            if (request == null || !request.Any())
                return false;

            var ids = request.Select(x => x.SubPlanId).ToList();

            var existingMaps = await _tecontext.PlanModuleMap
                .Where(x => ids.Contains(x.SubPlanId))
                .ToListAsync();

            foreach (var map in existingMaps)
            {
                var updated = request.First(x => x.SubPlanId == map.SubPlanId);

                map.PlanId = updated.PlanId;
                map.ModuleId = updated.ModuleId;
                map.Active = updated.Active;
            }

            await _tecontext.SaveChangesAsync();
            return true;
        }










        public async Task<PlanModuleMapResponseDto?> GetTemplePlanModuleByIdAsync(int subPlanId)
        {
            return await (
                from pm in _tecontext.PlanModuleMap
                join p in _tecontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _tecontext.Modules on pm.ModuleId equals m.ModuleId
                where pm.SubPlanId == subPlanId
                select new PlanModuleMapResponseDto
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

        public async Task<List<PlanModuleMapResponseDto>> GetTempleAllAsync()
        {
            return await (
                from pm in _tecontext.PlanModuleMap
                join p in _tecontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _tecontext.Modules on pm.ModuleId equals m.ModuleId
                select new PlanModuleMapResponseDto
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
        public async Task<int> CreateRentalPlanModuleAsync(XRS_PlanModuleMap request)
        {
            var map = new XRS_PlanModuleMap
            {
                PlanId = request.PlanId,
                ModuleId = request.ModuleId,
                Active = request.Active
            };

            _recontext.PlanModuleMap.Add(map);
            await _recontext.SaveChangesAsync();

            return map.SubPlanId;
        }

        public async Task<bool> UpdateRentalPlanModuleAsync(int subPlanId, XRS_PlanModuleMap request)
        {
            var map = await _recontext.PlanModuleMap.FindAsync(subPlanId);
            if (map == null) return false;

            map.PlanId = request.PlanId;
            map.ModuleId = request.ModuleId;
            map.Active = request.Active;

            await _recontext.SaveChangesAsync();
            return true;
        }

        public async Task<PlanModuleMapResponseDto?> GetRentalPlanModuleByIdAsync(int subPlanId)
        {
            return await (
                from pm in _recontext.PlanModuleMap
                join p in _recontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _recontext.Module on pm.ModuleId equals m.ModuleId
                where pm.SubPlanId == subPlanId
                select new PlanModuleMapResponseDto
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

        public async Task<List<PlanModuleMapResponseDto>> GetRentalAllAsync()
        {
            return await (
                from pm in _recontext.PlanModuleMap
                join p in _recontext.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _recontext.Module on pm.ModuleId equals m.ModuleId
                select new PlanModuleMapResponseDto
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
    }
}
#endregion



