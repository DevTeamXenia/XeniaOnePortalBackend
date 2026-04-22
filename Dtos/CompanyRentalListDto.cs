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
        public string? Pin { get; set; } // ✅ ADD THIS
        public string? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }

     
        public SubscriptionRentalSummaryDto? Subscription { get; set; }
    }
}
