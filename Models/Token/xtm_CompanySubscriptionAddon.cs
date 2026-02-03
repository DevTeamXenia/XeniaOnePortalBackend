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
            public int SubAddonId { get; set; }

            public int MainPlanId { get; set; }

            public int PlanId { get; set; }

            public int CompanyId { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal Amount { get; set; }

            public int DepCount { get; set; }

            [MaxLength(50)]
            public string? Status { get; set; }
        }
    }

}
