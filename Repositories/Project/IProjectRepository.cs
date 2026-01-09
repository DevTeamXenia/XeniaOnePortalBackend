
using XeniaRegistrationBackend.Dtos;

namespace XeniaRegistrationBackend.Repositories.Project
{
    public interface IProjectRepository
    {
        Task<IEnumerable<ProjectResponseDto>> GetAllAsync();


    }
}
