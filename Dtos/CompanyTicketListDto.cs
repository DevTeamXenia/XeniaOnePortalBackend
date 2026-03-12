namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTicketListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyType { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? Address { get; set; } = null!;

        public SubscriptionTicketSummaryDto? Subscription { get; set; }
    }
}
