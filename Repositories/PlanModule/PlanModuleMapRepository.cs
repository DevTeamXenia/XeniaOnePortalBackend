namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models.Temple;

    public class PlanModuleMapRepository : IPlanModuleMapRepository
    {
        private readonly TempleDbContext _context;

        public PlanModuleMapRepository(TempleDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreatePlanModuleAsync(TK_PlanModuleMap request)
        {
            var map = new TK_PlanModuleMap
            {
                PlanId = request.PlanId,
                ModuleId = request.ModuleId,
                Active = request.Active
            };

            _context.PlanModuleMap.Add(map);
            await _context.SaveChangesAsync();

            return map.SubPlanId;
        }

        public async Task<bool> UpdatePlanModuleAsync(int subPlanId, TK_PlanModuleMap request)
        {
            var map = await _context.PlanModuleMap.FindAsync(subPlanId);
            if (map == null) return false;

            map.PlanId = request.PlanId;
            map.ModuleId = request.ModuleId;
            map.Active = request.Active;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlanModuleMapResponseDto?> GetPlanModuleByIdAsync(int subPlanId)
        {
            return await (
                from pm in _context.PlanModuleMap
                join p in _context.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _context.Modules on pm.ModuleId equals m.ModuleId
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

        public async Task<List<PlanModuleMapResponseDto>> GetAllAsync()
        {
            return await (
                from pm in _context.PlanModuleMap
                join p in _context.SubscribePlan on pm.PlanId equals p.PlanId
                join m in _context.Modules on pm.ModuleId equals m.ModuleId
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
