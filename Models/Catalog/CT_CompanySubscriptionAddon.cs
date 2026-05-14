using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblCompanySubscriptionAddon", Schema = "dbo")]
    public class CT_CompanySubscriptionAddon
    {
        [Key]
        public int Id { get; set; }

        public int? MainPlanId { get; set; }    // FK to tblCompanySubscription.SubId

        public int? PlanId { get; set; }        // Addon plan

        public int? CompanyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }        // Actual amount charged

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DealerAmount { get; set; }  // Dealer rate

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal? CustomAmount { get; set; }  // Custom rate

        [MaxLength(50)]
        public string? RateType { get; set; }       // STANDARD / DEALER / CUSTOM

        public int? UserCount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "ACTIVE";  // ACTIVE / EXPIRED

        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}
