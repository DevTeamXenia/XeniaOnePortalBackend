namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTempleListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyType { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? Phone2 { get; set; }
        public string? RegNo { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? IFSCCode { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }
        public string? Address { get; set; } = null!;

        public SubscriptionTempleSummaryDto? Subscription { get; set; }
    }

}
