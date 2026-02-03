using System.ComponentModel.DataAnnotations;

namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTokenListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;

        public bool? Status { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }
        public SubscriptionTokenSummaryDto? Subscription { get; set; }
    }
}
