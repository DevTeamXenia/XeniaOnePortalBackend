namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionCatalogPlanResponseDto
    {

        public int SubPlanId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool Active { get; set; }
    }
     public class CatalogPlanModuleMapRequestDto
    {
        public int PlanId { get; set; }
        public int ModuleId { get; set; }
        public bool Active { get; set; }
    }

}
