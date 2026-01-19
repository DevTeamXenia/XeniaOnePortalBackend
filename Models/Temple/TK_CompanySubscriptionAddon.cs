using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_CompanySubscriptionAddon", Schema = "dbo")]
    public class TK_CompanySubscriptionAddon
    {
        [Key]
        [Column("subAddonId")]
        public int SubAddonId { get; set; }

        [Column("planId")]
        public int PlanId { get; set; }

        [Column("companyId")]
        public int CompanyId { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("userCount")]
        public int UserCount { get; set; }
    }
}
