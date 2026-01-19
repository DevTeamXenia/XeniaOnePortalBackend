using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models.Temple;
using XeniaRegistrationBackend.Repositories.Module;
using XeniaRegistrationBackend.Repositories.PlanModule;
using XeniaRegistrationBackend.Repositories.SubscriptionPlan;

namespace XeniaRegistrationBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/subscribe")]
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

        #region TEMPLE

        #region MODULE

        [HttpPost("module")]
        public async Task<IActionResult> CreateModule([FromBody] TK_Module request)
        {
            var id = await _moduleRepository.CreateModuleAsync(request);
            return Ok(new { Message = "Module created successfully", ModuleId = id });
        }


        [HttpPut("module/{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] TK_Module request)
        {
            var updated = await _moduleRepository.UpdateModuleAsync(id, request);
            if (!updated) return NotFound("Module not found");

            return Ok(new { Message = "Module updated successfully" });
        }


        [HttpGet("module")]
        public async Task<IActionResult> GetAllModule()
        {
            return Ok(await _moduleRepository.GetAllModuleAsync());
        }


        [HttpGet("module/{id}")]
        public async Task<IActionResult> GetModuleById(int id)
        {
            var module = await _moduleRepository.GetByIdModuleAsync(id);
            if (module == null) return NotFound("Module not found");

            return Ok(module);
        }

        #endregion

        #region SUBSCRIBTIONPLAN

        [HttpPost("plan")]
        public async Task<IActionResult> CreateSubscribePlan([FromBody] SubscribePlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateTempleSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("plan/{id}")]
        public async Task<IActionResult> CreateSubscribeUpdate(int id, [FromBody] SubscribePlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateTempleSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }

        
        [HttpGet("plan")]
        public async Task<IActionResult> GetAllSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllTempleSubscriptionPlanAsync());
        }

  
        [HttpGet("plan/{id}")]
        public async Task<IActionResult> GetlSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionTemplePlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }

        #endregion

        #region SUBSCRIBTIONPLANMODULE

        [HttpPost("planModuleMap")]
        public async Task<IActionResult> CreatePlanModule([FromBody] TK_PlanModuleMap request)
        {
            var id = await _planModuleMapRepository.CreatePlanModuleAsync(request);
            return Ok(new
            {
                Message = "Module mapped to plan successfully",
                SubPlanId = id
            });
        }

      
        [HttpPut("planModuleMap/{id}")]
        public async Task<IActionResult> UpdatePlanModule(int id, [FromBody] TK_PlanModuleMap request)
        {
            var updated = await _planModuleMapRepository.UpdatePlanModuleAsync(id, request);
            if (!updated) return NotFound("Mapping not found");

            return Ok(new { Message = "Mapping updated successfully" });
        }

       
        [HttpGet("planModuleMap/{id}")]
        public async Task<IActionResult> GetPlanModuleById(int id)
        {
            var data = await _planModuleMapRepository.GetPlanModuleByIdAsync(id);
            if (data == null) return NotFound("Mapping not found");

            return Ok(data);
        }

        [HttpGet("planModuleMap")]
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
            var subId = await _subscribePlanRepository.CreateTempleSubscriptionAsync(dto);

            return Ok(new
            {
                status = "success",
                subscriptionId = subId
            });
        }


        [HttpPost("addon")]
        public async Task<IActionResult> CreateAddon([FromBody] CompanySubscriptionAddonCreateDto dto)
        {
            var addonId = await _subscribePlanRepository.CreateTempleAddonAsync(dto);

            return Ok(new
            {
                status = "success",
                addonId = addonId
            });
        }

        #endregion

        #endregion


    }
}
