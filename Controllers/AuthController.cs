using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Repositories.Auth;


namespace XeniaRentalApi.Controllers
{

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
      
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

 

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var user = await _authRepository.AuthenticateUser(request);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "User does not exist."
                    });
                }

                var token = _authRepository.GenerateJwtToken(user);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful.",
                    Token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }




  


    }
}
