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
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repositoryCompanyRegistration.GetCompanyByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPut("temple/update")]
        public async Task<IActionResult> Update(UpdateCompanyDto dto)
        {
            await _repositoryCompanyRegistration.UpdateCompanyAsync(dto);
            return Ok(new { status = "updated" });
        }

        [HttpPost("temple/register")]
        public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegistrationRequestDto request)
        {
            var companyId = await _repositoryCompanyRegistration.RegisterCompanyAsync(request);

            return Ok(new
            {
                Status = "Success",
                CompanyId = companyId,
                Message = "Company registered successfully"
            });
        }

        #endregion


        #region TOKEN
/*
        [HttpGet("token")]
        public async Task<IActionResult> GetAll()
      => Ok(await _repositoryCompanyRegistration.GetAllTokenCompaniesAsync());

*/
        #endregion
    }

}
