namespace XeniaRegistrationBackend.Dtos
{
    public class ProjectResponseDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
