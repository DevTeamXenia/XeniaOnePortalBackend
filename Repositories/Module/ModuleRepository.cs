namespace XeniaRegistrationBackend.Repositories.Module
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Models;
    using XeniaTempleBackend.Models;

    public class ModuleRepository : IModuleRepository
    {
        private readonly TempleDbContext _context;

        public ModuleRepository(TempleDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateModuleAsync(TK_Module request)
        {
            var module = new TK_Module
            {
                ModuleName = request.ModuleName,
                ModuleDescription = request.ModuleDescription,
                ModuleActive = request.ModuleActive
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return module.ModuleId;
        }

        public async Task<bool> UpdateModuleAsync(int moduleId, TK_Module request)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null) return false;

            module.ModuleName = request.ModuleName;
            module.ModuleDescription = request.ModuleDescription;
            module.ModuleActive = request.ModuleActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TK_Module>> GetAllModuleAsync()
        {
            return await _context.Modules
                .Select(m => new TK_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .ToListAsync();
        }

        public async Task<TK_Module?> GetByIdModuleAsync(int moduleId)
        {
            return await _context.Modules
                .Where(m => m.ModuleId == moduleId)
                .Select(m => new TK_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .FirstOrDefaultAsync();
        }
    }

}
