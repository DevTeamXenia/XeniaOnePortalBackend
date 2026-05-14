namespace XeniaRegistrationBackend.Repositories.Module
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using XeniaRegistrationBackend.Models;
    using XeniaRegistrationBackend.Models.Catalog;
    using XeniaRegistrationBackend.Models.Rental;
    using XeniaRegistrationBackend.Models.Temple;

    public class ModuleRepository : IModuleRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly RentalDbContext _recontext;
        private readonly CatalogDbContext _cacontext;  

        public ModuleRepository(TempleDbContext tecontext, RentalDbContext recontext, CatalogDbContext cacontext)
        {
            _tecontext = tecontext;
            _recontext = recontext;
            _cacontext = cacontext;

        }

        public async Task<int> CreateTempleModuleAsync(TK_Module request)
        {
            var module = new TK_Module
            {
                ModuleName = request.ModuleName,
                ModuleDescription = request.ModuleDescription,
                ModuleActive = request.ModuleActive
            };

            _tecontext.Modules.Add(module);
            await _tecontext.SaveChangesAsync();

            return module.ModuleId;
        }

        public async Task<bool> UpdateTempleModuleAsync(int moduleId, TK_Module request)
        {
            var module = await _tecontext.Modules.FindAsync(moduleId);
            if (module == null) return false;

            module.ModuleName = request.ModuleName;
            module.ModuleDescription = request.ModuleDescription;
            module.ModuleActive = request.ModuleActive;

            await _tecontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TK_Module>> GetAllTempleModuleAsync()
        {
            return await _tecontext.Modules
                .Select(m => new TK_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .ToListAsync();
        }

        public async Task<TK_Module?> GetByIdTempleModuleAsync(int moduleId)
        {
            return await _tecontext.Modules
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



        public async Task<int> CreateRentalModuleAsync(XRS_Module request)
        {
            var module = new XRS_Module
            {
                ModuleName = request.ModuleName,
                ModuleDescription = request.ModuleDescription,
                ModuleActive = request.ModuleActive
            };

            _recontext.Module.Add(module);
            await _recontext.SaveChangesAsync();

            return module.ModuleId;
        }

        public async Task<bool> UpdateRentalModuleAsync(int moduleId, XRS_Module request)
        {
            var module = await _recontext.Module.FindAsync(moduleId);
            if (module == null) return false;

            module.ModuleName = request.ModuleName;
            module.ModuleDescription = request.ModuleDescription;
            module.ModuleActive = request.ModuleActive;

            await _recontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<XRS_Module>> GetAllRentalModuleAsync()
        {
            return await _recontext.Module
                .Select(m => new XRS_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .ToListAsync();
        }

        public async Task<XRS_Module?> GetByIdRentalModuleAsync(int moduleId)
        {
            return await _recontext.Module
                .Where(m => m.ModuleId == moduleId)
                .Select(m => new XRS_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .FirstOrDefaultAsync();
        }

        // ✅ ADD CATALOG MODULE METHODS
        public async Task<int> CreateCatalogModuleAsync(CT_Module request)
        {
            var module = new CT_Module
            {
                ModuleName = request.ModuleName,
                ModuleDescription = request.ModuleDescription,
                ModuleActive = request.ModuleActive
            };

            _cacontext.Modules.Add(module);
            await _cacontext.SaveChangesAsync();

            return module.ModuleId;
        }

        public async Task<bool> UpdateCatalogModuleAsync(int moduleId, CT_Module request)
        {
            var module = await _cacontext.Modules.FindAsync(moduleId);
            if (module == null) return false;

            module.ModuleName = request.ModuleName;
            module.ModuleDescription = request.ModuleDescription;
            module.ModuleActive = request.ModuleActive;

            await _cacontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CT_Module>> GetAllCatalogModuleAsync()
        {
            return await _cacontext.Modules
                .Select(m => new CT_Module
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleDescription = m.ModuleDescription,
                    ModuleActive = m.ModuleActive
                })
                .ToListAsync();
        }
        //public async Task<int> CreateCatalogModuleAsync(CT_Module request)
        //{
        //    var module = new CT_Module
        //    {
        //        ModuleName = request.ModuleName,
        //        ModuleDescription = request.ModuleDescription,
        //        ModuleActive = request.ModuleActive
        //    };

        //    _cacontext.Modules.Add(module);

        //    await _cacontext.SaveChangesAsync();

        //    return module.ModuleId;
        //}
        public async Task<CT_Module?> GetByIdCatalogModuleAsync(int moduleId)
        {
            return await _cacontext.Modules
                .Where(m => m.ModuleId == moduleId)
                .Select(m => new CT_Module
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


