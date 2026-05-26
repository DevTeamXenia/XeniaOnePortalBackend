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
            var ids = new List<int>();

            if (request == null || !request.Any())
                return ids;

            // Get all PlanIds from request
            var planIds = request.Select(x => x.PlanId).Distinct().ToList();

            // Remove all existing records for those PlanIds
            var existingRecords = await _tecontext.PlanModuleMap
                .Where(x => planIds.Contains(x.PlanId))
                .ToListAsync();

            if (existingRecords.Any())
            {
                _tecontext.PlanModuleMap.RemoveRange(existingRecords);
                await _tecontext.SaveChangesAsync();
            }

            // Insert new records
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

        //public async Task<bool> UpdateRentalPlanModuleAsync(int subPlanId, XRS_PlanModuleMap request)
        //{
        //    var map = await _recontext.PlanModuleMap.FindAsync(subPlanId);
        //    if (map == null) return false;

        //    map.PlanId = request.PlanId;
        //    map.ModuleId = request.ModuleId;
        //    map.Active = request.Active;

        //    await _recontext.SaveChangesAsync();
        //    return true;
        //}

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



