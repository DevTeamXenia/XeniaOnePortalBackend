using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Token
{
    [Table("xtm_CompanySubscription", Schema = "dbo")]
    public class xtm_CompanySubscription
    {
        [Key]
        [Column("subId")]
        public int SubId { get; set; }

        [Column("planId")]
        public int PlanId { get; set; }

        [Column("companyId")]
        public int CompanyId { get; set; }

        [Column("subscriptionDate")]
        public DateTime SubscriptionDate { get; set; }

        [Column("subscriptionStartDate")]
        public DateTime SubscriptionStartDate { get; set; }

        [Column("subscriptionEndDate")]
        public DateTime SubscriptionEndDate { get; set; }

        [Column("subscriptionAmount")]
        public decimal SubscriptionAmount { get; set; }

        [Column("subscriptionDays")]
        public int SubscriptionDays { get; set; }

        [Column("subscriptionDepCount")]
        public int SubscriptionDepCount { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = null!;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
