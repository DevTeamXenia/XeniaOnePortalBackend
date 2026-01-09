
using XeniaRegistrationBackend.Dtos;

namespace XeniaRegistrationBackend.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<UserAuthResponseDto?> AuthenticateUser(LoginRequestDto request);
        string GenerateJwtToken(UserAuthResponseDto user);


    }
}
