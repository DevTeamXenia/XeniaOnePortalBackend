using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Repositories.CompanyRegistration;

namespace XeniaRegistrationBackend.Controllers
{

    [AllowAnonymous]
    [Route("api/company")]
    [ApiController]
    public class CompanyRegistrationController : ControllerBase
    {
        private readonly ICompanyRegistrationRepository _repositoryCompanyRegistration;

        public CompanyRegistrationController(ICompanyRegistrationRepository repositoryCompanyRegistration)
        {
            _repositoryCompanyRegistration = repositoryCompanyRegistration;
        }

        #region TEMPLE


        [HttpGet("temple")]
        public async Task<IActionResult> GetAllTempleCompany()
        => Ok(await _repositoryCompanyRegistration.GetAllTempleCompaniesAsync());


        [HttpGet("temple/{id}")]
        public async Task<IActionResult> GetTempleCompanyByIdAsync(int id)
        {
            var result = await _repositoryCompanyRegistration.GetTempleCompanyByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPut("temple/update")]
        public async Task<IActionResult> UpdateTemple(UpdateTempleCompanyDto dto)
        {
            await _repositoryCompanyRegistration.UpdateTempleCompanyAsync(dto);
            return Ok(new { status = "updated" });
        }


        [HttpPost("temple/register")]
        public async Task<IActionResult> RegisterTempleCompanyAsync([FromBody] CompanyTempleRegistrationRequestDto request)
        {
            var companyId = await _repositoryCompanyRegistration.RegisterTempleCompanyAsync(request);

            return Ok(new
            {
                Status = "Success",
                CompanyId = companyId,
                Message = "Company registered successfully"
            });
        }

        #endregion


        #region TOKEN

        [HttpGet("token")]
        public async Task<IActionResult> GetAllTokenCompany()
         => Ok(await _repositoryCompanyRegistration.GetAllTokenCompaniesAsync());


        [HttpPost("token/register")]
        public async Task<IActionResult> RegisterTokenCompanyAsync([FromBody] CompanyTokenRegistrationRequestDto dto)
        {
            try
            {
                var companyId = await _repositoryCompanyRegistration.RegisterTokenCompanyAsync(dto);
                return Ok(new
                {
                    status = "success",
                    companyId
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


        [HttpPut("token/update")]
        public async Task<IActionResult> UpdateToken(UpdateTokenCompanyDto dto)
        {
            await _repositoryCompanyRegistration.UpdateTokenCompanyAsync(dto);
            return Ok(new { status = "updated" });
        }


        [HttpGet("token/{id}")]
        public async Task<IActionResult> GetTokenCompanyByIdAsync(int id)
        {
            var result = await _repositoryCompanyRegistration.GetTokenCompanyByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }




        #endregion


        #region RENTAL


        [HttpGet("rental")]
        public async Task<IActionResult> GetAllRentalCompany()
        => Ok(await _repositoryCompanyRegistration.GetAllRentalCompaniesAsync());


        [HttpGet("rental/{id}")]
        public async Task<IActionResult> GetRentalCompanyByIdAsync(int id)
        {
            var result = await _repositoryCompanyRegistration.GetRentalCompanyByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPut("rental/update")]
        public async Task<IActionResult> UpdateRental(UpdateRentalCompanyDto dto)
        {
            await _repositoryCompanyRegistration.UpdateRentalCompanyAsync(dto);
            return Ok(new { status = "updated" });
        }


        [HttpPost("rental/register")]
        public async Task<IActionResult> RegisterRentalCompanyAsync([FromBody] CompanyRentalRegistrationRequestDto request)
        {
            var companyId = await _repositoryCompanyRegistration.RegisterRentalCompanyAsync(request);

            return Ok(new
            {
                Status = "Success",
                CompanyId = companyId,
                Message = "Company registered successfully"
            });
        }

        #endregion
    }

}
