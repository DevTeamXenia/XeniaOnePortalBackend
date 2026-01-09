namespace XeniaRegistrationBackend.Dtos
{
    public class PlanModuleMapResponseDto
    {
        public int SubPlanId { get; set; }

        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;

        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = null!;

        public bool Active { get; set; }
    }

}
