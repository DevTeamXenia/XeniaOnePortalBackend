using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Repositories.Module;
using XeniaRegistrationBackend.Repositories.PlanModule;
using XeniaRegistrationBackend.Repositories.SubscriptionPlan;
using XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Controllers
{
    public class SubscribeController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ISubscribePlanRepository _subscribePlanRepository;
        private readonly IPlanModuleMapRepository _planModuleMapRepository;

        public SubscribeController(IModuleRepository moduleRepository, ISubscribePlanRepository subscribePlanRepository, IPlanModuleMapRepository planModuleMapRepository)
        {
            _moduleRepository = moduleRepository;
            _subscribePlanRepository = subscribePlanRepository;
            _planModuleMapRepository = planModuleMapRepository;
        }


        #region MODULE

        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] TK_Module request)
        {
            var id = await _moduleRepository.CreateModuleAsync(request);
            return Ok(new { Message = "Module created successfully", ModuleId = id });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] TK_Module request)
        {
            var updated = await _moduleRepository.UpdateModuleAsync(id, request);
            if (!updated) return NotFound("Module not found");

            return Ok(new { Message = "Module updated successfully" });
        }


        [HttpGet]
        public async Task<IActionResult> GetAllModule()
        {
            return Ok(await _moduleRepository.GetAllModuleAsync());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetModuleById(int id)
        {
            var module = await _moduleRepository.GetByIdModuleAsync(id);
            if (module == null) return NotFound("Module not found");

            return Ok(module);
        }

        #endregion

        #region SUBSCRIBTIONPLAN

        [HttpPost]
        public async Task<IActionResult> CreateSubscribePlan([FromBody] SubscribePlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> CreateSubscribeUpdate(int id, [FromBody] SubscribePlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllSubscriptionPlanAsync());
        }

  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetlSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionPlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }

        #endregion

        #region SUBSCRIBTIONPLANMODULE

        [HttpPost]
        public async Task<IActionResult> CreatePlanModule([FromBody] TK_PlanModuleMap request)
        {
            var id = await _planModuleMapRepository.CreatePlanModuleAsync(request);
            return Ok(new
            {
                Message = "Module mapped to plan successfully",
                SubPlanId = id
            });
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanModule(int id, [FromBody] TK_PlanModuleMap request)
        {
            var updated = await _planModuleMapRepository.UpdatePlanModuleAsync(id, request);
            if (!updated) return NotFound("Mapping not found");

            return Ok(new { Message = "Mapping updated successfully" });
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanModuleById(int id)
        {
            var data = await _planModuleMapRepository.GetPlanModuleByIdAsync(id);
            if (data == null) return NotFound("Mapping not found");

            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _planModuleMapRepository.GetAllAsync();
            return Ok(list);
        }

        #endregion

        #region SUBSCRIBTIONRENEW

        [HttpPost]
        public async Task<IActionResult> CreateSubscription([FromBody] CompanySubscriptionCreateDto dto)
        {
            var subId = await _subscribePlanRepository.CreateSubscriptionAsync(dto);

            return Ok(new
            {
                status = "success",
                subscriptionId = subId
            });
        }


        [HttpPost("addon")]
        public async Task<IActionResult> CreateAddon([FromBody] CompanySubscriptionAddonCreateDto dto)
        {
            var addonId = await _subscribePlanRepository.CreateAddonAsync(dto);

            return Ok(new
            {
                status = "success",
                addonId = addonId
            });
        }

        #endregion


    }
}
