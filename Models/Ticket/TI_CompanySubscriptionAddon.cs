using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models
{
    [Table("TI_CompanySubscriptionAddon", Schema = "dbo")]
    public class TI_CompanySubscriptionAddon
    {
        [Key]
        [Column("subAddonId")]
        public int SubAddonId { get; set; }

        [Column("companyId")]
        public int CompanyId { get; set; }

        [Column("mainPlanId")]
        public int MainPlanId { get; set; }

        [Column("planId")]
        public int PlanId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("userCount")]
        public int UserCount { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;
    }
}
