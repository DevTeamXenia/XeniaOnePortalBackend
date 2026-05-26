namespace XeniaRegistrationBackend.Models.Token
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace XeniaTempleBackend.Models
    {
        [Table("xtm_CompanySubscriptionAddon", Schema = "dbo")]
        public class xtm_CompanySubscriptionAddon
        {
            [Key]
            [Column("subAddonId")]
            public int SubAddonId { get; set; }

            [Column("planId")]
            public int PlanId { get; set; }

            [Column("mainPlanId")]
            public int MainPlanId { get; set; }

            [Column("companyId")]
            public int CompanyId { get; set; }

            [Column("amount")]
            public decimal Amount { get; set; }

            [Column("depCount")]
            public int DepCount { get; set; }

            [Column("status")]
            public string Status { get; set; }
        }
    }

}
