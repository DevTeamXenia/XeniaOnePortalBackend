using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRegistrationBackend.Repositories.Project;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _projectRepository.GetAllAsync();
        return Ok(projects);
    }
}
