using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Catalog;
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

  

        #region MODULE

        [HttpPost("temple/module")]
        public async Task<IActionResult> CreateTempleModule([FromBody] TK_Module request)
        {
            var id = await _moduleRepository.CreateTempleModuleAsync(request);
            return Ok(new { Message = "Module created successfully", ModuleId = id });
        }


        [HttpPut("temple/module/{id}")]
        public async Task<IActionResult> UpdateTempleModule(int id, [FromBody] TK_Module request)
        {
            var updated = await _moduleRepository.UpdateTempleModuleAsync(id, request);
            if (!updated) return NotFound("Module not found");

            return Ok(new { Message = "Module updated successfully" });
        }


        [HttpGet("temple/module")]
        public async Task<IActionResult> GetAllTempleModule()
        {
            return Ok(await _moduleRepository.GetAllTempleModuleAsync());
        }


        [HttpGet("temple/module/{id}")]
        public async Task<IActionResult> GetTempleModuleById(int id)
        {
            var module = await _moduleRepository.GetByIdTempleModuleAsync(id);
            if (module == null) return NotFound("Module not found");

            return Ok(module);
        }



        [HttpPost("rental/module")]
        public async Task<IActionResult> CreateRentalModule([FromBody] XRS_Module request)
        {
            var id = await _moduleRepository.CreateRentalModuleAsync(request);
            return Ok(new { Message = "Module created successfully", ModuleId = id });
        }


        [HttpPut("rental/module/{id}")]
        public async Task<IActionResult> UpdateRentalModule(int id, [FromBody] XRS_Module request)
        {
            var updated = await _moduleRepository.UpdateRentalModuleAsync(id, request);
            if (!updated) return NotFound("Module not found");

            return Ok(new { Message = "Module updated successfully" });
        }


        [HttpGet("rental/module")]
        public async Task<IActionResult> GetRentalAllModule()
        {
            return Ok(await _moduleRepository.GetAllRentalModuleAsync());
        }


        [HttpGet("rental/module/{id}")]
        public async Task<IActionResult> GetRentalModuleById(int id)
        {
            var module = await _moduleRepository.GetByIdRentalModuleAsync(id);
            if (module == null) return NotFound("Module not found");

            return Ok(module);
        }




        [HttpPost("xeniaOne/module")]
        public async Task<IActionResult> CreateCatalogModule([FromBody] CT_Module request)
        {
            var id = await _moduleRepository.CreateCatalogModuleAsync(request);

            return Ok(new
            {
                Message = "Module created successfully",
                ModuleId = id
            });
        }

        [HttpPut("xeniaOne/module/{id}")]
        public async Task<IActionResult> UpdateCatalogModule(int id, [FromBody] CT_Module request)
        {
            var updated = await _moduleRepository.UpdateCatalogModuleAsync(id, request);

            if (!updated)
                return NotFound("Module not found");

            return Ok(new
            {
                Message = "Module updated successfully"
            });
        }

        [HttpGet("xeniaOne/module")]
        public async Task<IActionResult> GetAllCatalogModule()
        {
            return Ok(await _moduleRepository.GetAllCatalogModuleAsync());
        }

        [HttpGet("xeniaOne/module/{id}")]
        public async Task<IActionResult> GetCatalogModuleById(int id)
        {
            var module = await _moduleRepository.GetByIdCatalogModuleAsync(id);
            if (module == null) return NotFound("Module not found");

            return Ok(module);
        }

        [HttpPost("xeniaOne/planModuleMap")]
        public async Task<IActionResult> CreateCatalogPlanModule([FromBody] List<CT_PlanModuleMap> request)
        {
            var ids = await _planModuleMapRepository.CreateCatalogPlanModuleAsync(request);

            return Ok(new
            {
                Message = "Modules mapped to plan successfully",
                SubPlanIds = ids
            });
        }


        [HttpPut("xeniaOne/planModuleMap/{id}")]
        public async Task<IActionResult> UpdateCatalogPlanModule(int id, [FromBody] List<CT_PlanModuleMap> request)
        {
            var updated = await _planModuleMapRepository.UpdateCatalogPlanModuleAsync(request);

            if (!updated)
                return NotFound("Mapping not found");

            return Ok(new
            {
                Message = "Mapping updated successfully"
            });
        }


   

        [HttpGet("xeniaOne/planModuleMap")]
        public async Task<IActionResult> GetAllCatalogPlanModules()
        {
            var list = await _planModuleMapRepository.GetCatalogAllAsync();
            return Ok(list);
        }


        [HttpGet("xeniaOne/planModuleMap/{id}")]
        public async Task<IActionResult> GetCatalogPlanModuleById(int id)
        {
            var data = await _planModuleMapRepository.GetCatalogPlanModuleByIdAsync(id);

            if (data == null)
                return NotFound("Mapping not found");

            return Ok(data);
        }


        #endregion


        #region SUBSCRIBTIONPLAN

        [HttpPost("temple/plan")]
        public async Task<IActionResult> CreateTempleSubscribePlan([FromBody] SubscribeTemplePlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateTempleSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("temple/plan/{id}")]
        public async Task<IActionResult> CreateTempleSubscribeUpdate(int id, [FromBody] SubscribeTemplePlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateTempleSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }

        
        [HttpGet("temple/plan")]
        public async Task<IActionResult> GetAllTempleSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllTempleSubscriptionPlanAsync());
        }

  
        [HttpGet("temple/plan/{id}")]
        public async Task<IActionResult> GetTempleSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionTemplePlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }



        [HttpPost("token/plan")]
        public async Task<IActionResult> CreateTokenSubscribePlan([FromBody] SubscribeTokenPlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateTokenSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("token/plan/{id}")]
        public async Task<IActionResult> CreateTokenSubscribeUpdate(int id, [FromBody] SubscribeTokenPlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateTokenSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }


        [HttpGet("token/plan")]
        public async Task<IActionResult> GetAllTokenSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllTokenSubscriptionPlanAsync());
        }


        [HttpGet("token/plan/{id}")]
        public async Task<IActionResult> GetTokenSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionTokenPlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }



        [HttpPost("rental/plan")]
        public async Task<IActionResult> CreateRentalSubscribePlan([FromBody] SubscribeRentalPlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateRentalSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("rental/plan/{id}")]
        public async Task<IActionResult> CreateRentalSubscribeUpdate(int id, [FromBody] SubscribeRentalPlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateRentalSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }
         

        [HttpGet("rental/plan")]
        public async Task<IActionResult> GetAllRentalSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllRentalSubscriptionPlanAsync());
        }


        [HttpGet("rental/plan/{id}")]
        public async Task<IActionResult> GetRentalSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionRentalPlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }



        [HttpPost("ticket/plan")]
        public async Task<IActionResult> CreateTicketSubscribePlan([FromBody] SubscribeTicketPlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateTicketSubscribePlanAsync(request);

            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }


        [HttpPut("ticket/plan/{id}")]
        public async Task<IActionResult> CreateTicketSubscribeUpdate(int id, [FromBody] SubscribeTicketPlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.CreateTicketSubscribeUpdateAsync(id, request);

            if (!updated) return NotFound("Plan not found");

            return Ok(new { Message = "Plan updated successfully" });
        }


        [HttpGet("ticket/plan")]
        public async Task<IActionResult> GetAllTicketSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllTicketSubscriptionPlanAsync());
        }


        [HttpGet("ticket/plan/{id}")]
        public async Task<IActionResult> GetTicketSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionTicketPlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");

            return Ok(plan);
        }

        #endregion


        #region SUBSCRIBTIONPLANMODULE

        [HttpPost("temple/planModuleMap")]
        public async Task<IActionResult> CreateTemplePlanModule([FromBody] List<TK_PlanModuleMap> request)
        {
            var ids = await _planModuleMapRepository.CreateTemplePlanModuleAsync(request);

            return Ok(new
            {
                Message = "Modules mapped to plan successfully",
                SubPlanIds = ids
            });
        }


        [HttpPut("temple/planModuleMap/{id}")]
        public async Task<IActionResult> UpdateTemplePlanModule(int id, [FromBody] List<TK_PlanModuleMap> request)
        {
            var updated = await _planModuleMapRepository.UpdateTemplePlanModuleAsync(request);
            if (!updated) return NotFound("Mapping not found");

            return Ok(new { Message = "Mapping updated successfully" });
        }

       
        [HttpGet("temple/planModuleMap/{id}")]
        public async Task<IActionResult> GetTemplePlanModuleById(int id)
        {
            var data = await _planModuleMapRepository.GetTemplePlanModuleByIdAsync(id);
            if (data == null) return NotFound("Mapping not found");

            return Ok(data);
        }


        [HttpGet("temple/planModuleMap")]
        public async Task<IActionResult> GetTempleAll()
        {
            var list = await _planModuleMapRepository.GetTempleAllAsync();
            return Ok(list);
        }


        [HttpPost("rental/planModuleMap")]
        public async Task<IActionResult> CreateRentalPlanModule([FromBody] XRS_PlanModuleMap request)
        {
            var id = await _planModuleMapRepository.CreateRentalPlanModuleAsync(request);
            return Ok(new
            {
                Message = "Module mapped to plan successfully",
                SubPlanId = id
            });
        }


        [HttpPut("rental/planModuleMap/{id}")]
        public async Task<IActionResult> UpdateRentalPlanModule(int id, [FromBody] XRS_PlanModuleMap request)
        {
            var updated = await _planModuleMapRepository.UpdateRentalPlanModuleAsync(id, request);
            if (!updated) return NotFound("Mapping not found");

            return Ok(new { Message = "Mapping updated successfully" });
        }


        [HttpGet("rental/planModuleMap/{id}")]
        public async Task<IActionResult> GetRentalPlanModuleById(int id)
        {
            var data = await _planModuleMapRepository.GetRentalPlanModuleByIdAsync(id);
            if (data == null) return NotFound("Mapping not found");

            return Ok(data);
        }


        [HttpGet("rental/planModuleMap")]
        public async Task<IActionResult> GetRentalAll()
        {
            var list = await _planModuleMapRepository.GetRentalAllAsync();
            return Ok(list);
        }

        #endregion


        #region SUBSCRIBTIONRENEW

        [HttpPost("temple/renew")]
        public async Task<IActionResult> CreateTempleSubscription([FromBody] CompanyTempleSubscriptionCreateDto dto)
        {
            var subId = await _subscribePlanRepository.CreateTempleSubscriptionAsync(dto);

            return Ok(new
            {
                status = "success",
                subscriptionId = subId
            });
        }


        [HttpPost("temple/addon")]
        public async Task<IActionResult> CreateTempleAddon([FromBody] CompanyTempleSubscriptionAddonCreateDto dto)
        {
            var addonId = await _subscribePlanRepository.CreateTempleAddonAsync(dto);

            return Ok(new
            {
                status = "success",
                addonId = addonId
            });
        }


        [HttpPost("token/renew")]
        public async Task<IActionResult> CreateTokenSubscription([FromBody] CompanyTokenSubscriptionCreateDto dto)
        {
            var subId = await _subscribePlanRepository.CreateTokenSubscriptionAsync(dto);

            return Ok(new
            {
                status = "success",
                subscriptionId = subId
            });
        }


        [HttpPost("token/addon")]
        public async Task<IActionResult> CreateTokenAddon([FromBody] CompanyTokenSubscriptionAddonCreateDto dto)
        {
            var addonId = await _subscribePlanRepository.CreateTokenAddonAsync(dto);

            return Ok(new
            {
                status = "success",
                addonId = addonId
            });
        }


        [HttpPost("rental/renew")]
        public async Task<IActionResult> CreateRentalSubscription([FromBody] CompanyRentalSubscriptionCreateDto dto)
        {
            var subId = await _subscribePlanRepository.CreateRentalSubscriptionAsync(dto);

            return Ok(new
            {
                status = "success",
                subscriptionId = subId
            });
        }
        
        [HttpPost("rental/addon")]
public async Task<IActionResult> CreateRentalAddon([FromBody] CompanyRentalSubscriptionAddonCreateDto dto)
{
    var addonId = await _subscribePlanRepository.CreateRentalAddonAsync(dto);

    return Ok(new
    {
        status = "success",
        addonId = addonId
    });
}
        [HttpPost("ticket/renew")]
        public async Task<IActionResult> CreateTicketSubscription([FromBody] CompanyTicketSubscriptionCreateDto dto)
        {
            var subId = await _subscribePlanRepository.CreateTicketSubscriptionAsync(dto);
            return Ok(new { status = "success", subscriptionId = subId });
        }


        #endregion

        #region XENIAOne

        [HttpPost("xeniaOne/plan")]
        public async Task<IActionResult> CreateCatalogSubscribePlan([FromBody] SubscribeCataloguePlanRequestDto request)
        {
            var id = await _subscribePlanRepository.CreateCatalogSubscribePlanAsync(request);
            return Ok(new { Message = "Plan created successfully", PlanId = id });
        }

        [HttpPut("xeniaOne/plan/{id}")]
        public async Task<IActionResult> UpdateCatalogSubscribePlan(int id, [FromBody] SubscribeCataloguePlanRequestDto request)
        {
            var updated = await _subscribePlanRepository.UpdateCatalogSubscribePlanAsync(id, request);
            if (!updated) return NotFound("Plan not found");
            return Ok(new { Message = "Plan updated successfully" });
        }

        [HttpGet("xeniaOne/plan")]
        public async Task<IActionResult> GetAllCatalogSubscriptionPlan()
        {
            return Ok(await _subscribePlanRepository.GetAllCatalogSubscriptionPlanAsync());
        }

        [HttpGet("xeniaOne/plan/{id}")]
        public async Task<IActionResult> GetCatalogSubscriptionPlanById(int id)
        {
            var plan = await _subscribePlanRepository.GetSubscriptionCatalogPlanByIdAsync(id);
            if (plan == null) return NotFound("Plan not found");
            return Ok(plan);
        }

        [HttpPost("xeniaOne/renew")]
        public async Task<IActionResult> CreateCatalogSubscription(
     [FromBody] CompanyCatalogSubscriptionCreateDto dto)
        {
            try
            {
                var subId = await _subscribePlanRepository
                    .CreateCatalogSubscriptionAsync(dto);

                return Ok(new
                {
                    status = "success",
                    message = "Subscription created successfully",
                    subscriptionId = subId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        [HttpPost("xeniaOne/addon")]
        public async Task<IActionResult> CreateCatalogAddon(
      [FromBody] CompanyCatalogSubscriptionAddonCreateDto dto)
        {
            try
            {
                var addonId = await _subscribePlanRepository.CreateCatalogAddonAsync(dto);

                return Ok(new
                {
                    status = "success",
                    addonId = addonId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        [HttpGet("xeniaOne/summary/{companyId}")]
        public async Task<IActionResult> GetCatalogSubscriptionSummary(int companyId)
        {
            var summary = await _subscribePlanRepository.GetCatalogSubscriptionSummaryAsync(companyId);
            if (summary == null) return NotFound("No subscription found");
            return Ok(summary);
        }

        //[HttpGet("catalog/special-rate/company/{companyId}")]
        //public async Task<IActionResult> GetCatalogSpecialRatesByCompany(int companyId)
        //{
        //    var rates = await _subscribePlanRepository
        //        .GetCatalogSpecialRatesByCompanyAsync(companyId);
        //    return Ok(rates);
        //}

        #endregion


    }
}
