namespace XeniaRegistrationBackend.Repositories.Project
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models;

    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync()
        {
            return await _context.Projects
                .Where(p => p.IsActive)
                .Select(p => new ProjectResponseDto
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                })
                .ToListAsync();
        }
    }

}
