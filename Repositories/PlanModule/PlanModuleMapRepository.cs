namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models;
    using XeniaRegistrationBackend.Models.Rental;
    using XeniaRegistrationBackend.Models.Temple;

    public class PlanModuleMapRepository : IPlanModuleMapRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly RentalDbContext _recontext;

        public PlanModuleMapRepository(TempleDbContext tecontext, RentalDbContext recontext)
        {
            _tecontext = tecontext;
            _recontext = recontext; 
        }

        public async Task<int> CreateTemplePlanModuleAsync(TK_PlanModuleMap request)
        {
            var map = new TK_PlanModuleMap
            {
                PlanId = request.PlanId,
                ModuleId = request.ModuleId,
                Active = request.Active
            };

            _tecontext.PlanModuleMap.Add(map);
            await _tecontext.SaveChangesAsync();

            return map.SubPlanId;
        }

        public async Task<bool> UpdateTemplePlanModuleAsync(int subPlanId, TK_PlanModuleMap request)
        {
            var map = await _tecontext.PlanModuleMap.FindAsync(subPlanId);
            if (map == null) return false;

            map.PlanId = request.PlanId;
            map.ModuleId = request.ModuleId;
            map.Active = request.Active;

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
