namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string CompanyType { get; set; } = null!;
        public string District { get; set; } = null!;
        public string State { get; set; } = null!;

        public SubscriptionSummaryDto? Subscription { get; set; }
    }

}
