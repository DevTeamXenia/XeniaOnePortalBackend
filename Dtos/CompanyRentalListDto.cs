namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyRentalListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;

        public bool? IsActive { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public SubscriptionRentalSummaryDto? Subscription { get; set; }
    }
}
