namespace XeniaRegistrationBackend.Dtos
{
    // ── Single item response ───────────────────────
    public class PlanModuleMapResponseDto
    {
        public int SubPlanId { get; set; }
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public bool Active { get; set; }
    }

    // ── Grouped response — Plan with Multiple Modules
    public class PlanModuleGroupResponseDto
    {
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public List<ModuleItemDto> Modules { get; set; } = new();
    }

    public class ModuleItemDto
    {
        public int SubPlanId { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public bool Active { get; set; }
    }

    // ── Create Multiple Modules Request ────────────
    public class CreatePlanModuleMapBulkDto
    {
        public int PlanId { get; set; }
        public List<ModuleMapItemDto> Modules { get; set; } = new();
    }

    public class ModuleMapItemDto
    {
        public int ModuleId { get; set; }
        public bool Active { get; set; } = true;
    }

    // ── Update Multiple Modules Request ────────────
    public class UpdatePlanModuleMapBulkDto
    {
        public int PlanId { get; set; }
        public List<UpdateModuleMapItemDto> Modules { get; set; } = new();
    }

    public class UpdateModuleMapItemDto
    {
        public int SubPlanId { get; set; }
        public int ModuleId { get; set; }
        public bool Active { get; set; }
    }
}